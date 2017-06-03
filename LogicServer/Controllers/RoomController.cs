using LogicServer.Data;
using Microsoft.ServiceFabric.Data;
using Model;
using Model.Data.Account;
using Model.Data.Npc;
using Model.Protocol;
using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Controllers
{
    public class RoomController : BaseInstance<RoomController>
    {
        IReliableStateManager SM { get { return LogicServer.Instance.StateManager; } }

        /// <summary>
        /// 获得房间信息
        /// </summary>
        public async Task<BaseResponseData> GetRoom()
        {
            return await GetRoom(LogicServer.User.role.Id);
        }
        /// <summary>
        /// 获得房间信息
        /// </summary>
        public async Task<BaseResponseData> GetRoom(Guid roleId)
        {
            RoomResult result = new RoomResult();
            var data = await Data.Helper.RoomData.Instance.GetRoomData();
            if (data == null)
            {
                //初始化
                data = GetNewRoom(roleId.ToString(), 1);

                await Data.Helper.RoomData.Instance.UpdateRoomData(data);
            }

            result.RoleId = data.RoleId;
            result.RoomId = data.RoomId;
            result.Config = data.Config;
            return result;
        }

        public async Task<BaseResponseData> OnRoomBuyReq()
        {
            RoomBuyResult result = new RoomBuyResult();

            var user = LogicServer.User;

            var req = await InitHelpers.GetPse().DeserializeAsync<RoomBuyReq>(user.bytes);

            var currRoom = await Data.Helper.RoomData.Instance.GetRoomData();
            //升级,不需要客户端传入
            var roomId = currRoom.RoomId + 1;

            result.roomId = roomId;

            //同样的?
            if (roomId == currRoom.RoomId)
            {
                return result;
            }

            var roleId = user.role.Id.ToString();

            var newRoom = GetNewRoom(roleId, roomId);//新房间
            if (currRoom == null || newRoom == null)
            {
                result.Result = GameEnum.WsResult.Error;
            }
            else
            {
                result.Result = await ChangeRoom(currRoom, newRoom);
            }

            return result;
        }

        /// <summary>
        /// 房间改变
        /// </summary>
        /// <param name="newRoom"></param>
        private async Task<GameEnum.WsResult> ChangeRoom(Room currRoom, Room newRoom)
        {
            var user = LogicServer.User;
            var result = GameEnum.WsResult.Success;

            //同样的?
            if (newRoom.RoomId == currRoom.RoomId)
            {
                return result;
            }

            //是否有足够的货币
            var roominfo = AutoData.Room.GetForKey(newRoom.RoomId);
            var has = BagController.Instance.CheckMoney((long)roominfo.Cost.Count, roominfo.Cost.ItemId);
            if (!has)
            {
                result = GameEnum.WsResult.NotEnoughMoney;
                return result;
            }

            //卖掉房间减少身价
            var cost = -AutoData.Room.GetForKey(currRoom.RoomId).Status;
            //增长的身份
            cost += roominfo.Status;

            ////--------------
            //消耗货币,通知减少
            if (roominfo.Cost.Count != 0)
            {
                await BagController.Instance.UseItemsAsync(SM, user.role.Id, roominfo.Cost.ItemId, (long)roominfo.Cost.Count);//db msg
            }

            //把当前的家具添加到背包中
            foreach (var itemId in currRoom.Config.Values)
            {
                await BagController.Instance.AddItemToRoleBag(itemId, 1);//db
            }

            //获得新房间
            await Data.Helper.RoomData.Instance.UpdateRoomData(newRoom);//db

            user.role.SocialStatus += cost;
            await MsgSender.Instance.UpdateIncome();//db


            ////---------------
            //通知房间信息更新
            var send = await GetRoom(user.role.Id);
            await MsgMaker.SendMessage(WSResponseMsgID.RoomResult, send);//msg

            return result;
        }

        /// <summary>
        /// 获得一个新的房间,
        /// </summary>
        public Room GetNewRoom(string roleId, int roomId)
        {
            var room = AutoData.Room.GetForKey(roomId);
            if (room == null)
                return null;

            var data = new Room();
            data.RoleId = roleId;
            data.RoomId = roomId;
            data.Config = new Dictionary<int, int>();

            foreach (var fur in room.Furniture)
            {
                data.Config[fur.Point] = fur.ItemID;
            }
            return data;
        }

        public async Task<BaseResponseData> OnRoomConfigUpdateReq()
        {
            RoomConfigUpdateResult result = new RoomConfigUpdateResult();
            var user = LogicServer.User;
            var req = await InitHelpers.GetPse().DeserializeAsync<RoomConfigUpdateReq>(user.bytes);
            if (req.Config == null)
                return result;

            ///获得原来的家具信息
            ///得到当前更新房间需要购买的新的家具
            ///如果有需要购买的家具，则先进行购买
            ///
            ///购买后开始更换家具
            ///更新掉的家具放到背包中
            ///没有新的家具从背包中移除
            ///更新房间的相应配置
            ///

            //变更了配置的位置
            var changeConfig = new List<int>();

            var room = await Data.Helper.RoomData.Instance.GetRoomData();
            var oldfur = new Dictionary<int, int>();
            //之前的配置家具数量
            foreach (var config in room.Config)
            {
                if (!oldfur.ContainsKey(config.Value))
                {
                    oldfur.Add(config.Value, 0);
                }
                oldfur[config.Value] += 1;
            }
            //新的家具配置数量
            var newfur = new Dictionary<int, int>();
            foreach (var config in req.Config)
            {
                if (!newfur.ContainsKey(config.Value))
                {
                    newfur.Add(config.Value, 0);
                }
                newfur[config.Value] += 1;

                if (config.Value != room.Config[config.Key])
                {
                    changeConfig.Add(config.Key);
                }
            }
            //BagController.Instance.GetMoney()
            var buys = new Dictionary<int, int>();
            var costs = new Dictionary<int, int>();//购买消耗
            foreach (var kv in newfur)
            {
                var itemId = kv.Key;
                var count = kv.Value;
                var baseCount = oldfur.ContainsKey(itemId) ? oldfur[itemId] : 0;
                count = baseCount > count ? 0 : count - baseCount;

                //不足再从背包里找
                if (count > 0)
                {
                    var bagCount = (int)BagController.Instance.GetItemCount(itemId);
                    count = bagCount > count ? 0 : count - bagCount;
                }
                if (count > 0)
                {
                    buys.Add(itemId, count);
                    var itemdata = AutoData.Item.GetForId(itemId);
                    if (itemdata.Cost.CurrencyID != 0)
                    {
                        if (!costs.ContainsKey(itemdata.Cost.CurrencyID))
                        {
                            costs.Add(itemdata.Cost.CurrencyID, 0);
                        }
                        costs[itemdata.Cost.CurrencyID] += itemdata.Cost.Count;
                    }
                }
            }
            ///购买,是否有足够的货币
            foreach (var kv in costs)
            {
                var itemId = kv.Key;
                var count = kv.Value;
                if (!BagController.Instance.CheckMoney(count, itemId))
                {
                    result.Result = GameEnum.WsResult.NotEnoughMoney;
                    return result;
                }
            }
            ///消耗货币
            foreach (var kv in costs)
            {
                var itemId = kv.Key;
                var count = kv.Value;
                await BagController.Instance.UseItemsAsync(SM, user.role.Id, itemId, count);
            }
            ///获得道具
            foreach (var kv in buys)
            {
                await BagController.Instance.AddItemToRoleBag(kv.Key, kv.Value);
            }
            ///更换的道具添加到背包中,其它的直接更新到装配界面中,并从背包中移除
            foreach (var furPoint in changeConfig)
            {
                var old = room.Config[furPoint];
                var newx = req.Config[furPoint];
                //旧的进背包,
                await BagController.Instance.AddItemToRoleBag(old, 1);
                //新的从背包中移除
                if (!await BagController.Instance.RemoveItemsAsync(newx, 1))
                {
                    result.Result = GameEnum.WsResult.NotEnoughItem;
                    return result;
                }
            }

            //更新房间配置
            foreach (var config in req.Config)
            {
                room.Config[config.Key] = config.Value;
            }
            await Data.Helper.RoomData.Instance.UpdateRoomData(room);

            result.Config = req.Config;//只返回更新的

            return result;
        }

        public async Task<BaseResponseData> OnRoomSellReq()
        {
            RoomSellResult result = new RoomSellResult();

            var user = LogicServer.User;

            var req = await InitHelpers.GetPse().DeserializeAsync<RoomBuyReq>(user.bytes);

            var currRoom = await Data.Helper.RoomData.Instance.GetRoomData();
            //出售后回到最初的住宅
            var roomId = 1;

            //同样的?
            if (roomId == currRoom.RoomId)
            {
                return result;
            }

            var roleId = user.role.Id.ToString();
            var newRoom = GetNewRoom(roleId, roomId);//新房间
            if (currRoom == null || newRoom == null)
            {
                result.Result = GameEnum.WsResult.Error;
            }
            else
            {
                result.Result = await ChangeRoom(currRoom, newRoom);
            }

            return result;
        }

        public async Task<BaseResponseData> OnRoomVisitReq()
        {
            RoomVisitResult result = new RoomVisitResult();

            return result;
        }
    }
}