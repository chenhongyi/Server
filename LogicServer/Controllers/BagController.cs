using Microsoft.ServiceFabric.Data;
using Model;
using Model.ResponseData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameEnum;
using Model.ViewModels;
using Shared;
using static AutoData.Item;
using Model.Data.Business;
using LogicServer.Data;
using LogicServer.Data.Helper;

namespace LogicServer.Controllers
{
    public class BagController : BaseInstance<BagController>
    {

        public BagController()
        {
            TxtReader.Init();
        }

        public long GetMoney(int moneyType)
        {
            LogicServer.User.bag.Items.TryGetValue(moneyType, out Model.Data.General.Item money);
            return money.CurCount;
        }

        public bool CheckMoney(long money, int moneyType)
        {
            if (GetMoney(moneyType) >= money)
                return true;
            return false;
        }

        public async Task<ChangeAvatarResult> ChangeAvatar(int[] id, ChangeAvatarResult result)
        {
            var role = LogicServer.User.role;
            #region 检查时装部位
            var parts = GetAvatarParts(role.Avatar);
            List<int> oldAvatar = new List<int>();  //换下来的时装id
            #endregion

            foreach (var i in id)
            {
                if (!(CheckItemInBag(i, 1))) { result.Result = GameEnum.WsResult.NotItemInBag; return result; }
                var tmp = GetForId(i);
                if (tmp == null) { result.Result = GameEnum.WsResult.NotItemConfig; return result; }
                oldAvatar.AddRange(GetOldAvatarItemId(parts, tmp.Parts));
                foreach (var j in tmp.Attribute)    //增加属性
                {
                    var attr = role.UserAttr.First(a => a.UserAttrID == j.AttributeID);
                    checked
                    {
                        try
                        {
                            attr.Count += j.Count;
                        }
                        catch (OverflowException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }

                }
            }
            foreach (var i in oldAvatar.Distinct())   //移除旧的属性
            {
                var tmp = GetForId(i);
                foreach (var item in tmp.Attribute)
                {
                    var attr = role.UserAttr.First(a => a.UserAttrID == item.AttributeID);
                    checked
                    {
                        try
                        {
                            attr.Count -= item.Count;
                        }
                        catch (OverflowException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            //检查背包剩余空间是否可以存放卸下来的时装
            foreach (var oldItem in oldAvatar)
            {
                if (!CheckBagSpace(oldItem, 1))
                {
                    result.Result = GameEnum.WsResult.NotEnoughBagSpace;
                    return result;
                }
            }

            RoleDataHelper.Instance.UpdateRoleByRoleIdAsync(role.Id, role).Wait();


            //移除背包中的时装
            foreach (var item in id)
            {
                await RemoveItemsAsync(item, 1);
            }
            foreach (var item in oldAvatar)
            {
                await AddItemToRoleBag(item, 1);  //增加旧时装到背包中
            }
            //更换时装到身上
            await UpdateAvaterAsync(id, parts);

            ///构造返回
            var roleAttrList = await RoleAttrListDataHelper.Instance.GetRoleAttrByRoleId(role.Id);
            if (roleAttrList != null)
            {
                foreach (var art in roleAttrList)
                {
                    result.ChangeAttr.Add(new Model.ResponseData.UserAttr()
                    {
                        Count = art.Count,
                        UserAttrID = art.UserAttrID
                    });
                }
            }

            result.BagInfo.CurUsedCell = LogicServer.User.bag.CurUsedCell;
            result.BagInfo.MaxCellNumber = LogicServer.User.bag.MaxCellNumber;
            foreach (var item in LogicServer.User.bag.Items)
            {
                result.BagInfo.Items.Add(item.Key, new LoadRoleBagInfo()
                {
                    CurCount = item.Value.CurCount,
                    OnSpace = item.Value.OnSpace
                });
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">道具集合</param>
        /// <param name="oldAvatar">key=部位  value=道具id</param>
        /// <returns></returns>
        private async Task UpdateAvaterAsync(int[] id, Dictionary<int, int> oldAvatar)
        {
            var role = LogicServer.User.role;
            UpdateAvatarResult result = new UpdateAvatarResult();
            foreach (var i in oldAvatar)
            {
                foreach (var j in id)
                {
                    var tmp = GetItemById(j);
                    if (tmp.Parts.Contains(i.Key))
                    {
                        role.Avatar.Remove(i.Value);
                        if (!role.Avatar.Contains(j))
                        {
                            role.Avatar.Add(j);
                            result.Id.Add(j);
                        }
                    }
                }
            }
            await RoleDataHelper.Instance.UpdateRoleByRoleIdAsync(role.Id, role);
            var data = await InitHelpers.GetPse().SerializeAsync(result);
            await MsgMaker.SendMessage(WSResponseMsgID.UpdateAvatarResult, 1, data);
        }

        /// <summary>
        /// 拿到即将被替换的部位的时装的id
        /// </summary>
        /// <param name="dic">存储部位对应时装道具id的字典（1对多）  部位 ----  itemId </param>
        /// <param name="parts">道具的部位int集合</param>
        /// <returns></returns>
        private static List<int> GetOldAvatarItemId(Dictionary<int, int> dic, List<int> parts)
        {
            List<int> result = new List<int>();
            foreach (var k in parts)
            {
                if (dic.ContainsKey(k))
                {
                    var oldAvatarId = 0;
                    if (dic.TryGetValue(k, out oldAvatarId))
                    {
                        result.Add(oldAvatarId);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 构造背包数据
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public Dictionary<int, LoadRoleBagInfo> GetRoleItems(Dictionary<int, Model.Data.General.Item> items)
        {
            var result = new Dictionary<int, LoadRoleBagInfo>();
            if (items.Any())
            {
                foreach (var item in items)
                {
                    result.Add(item.Key, new LoadRoleBagInfo()
                    {
                        CurCount = item.Value.CurCount,
                        OnSpace = item.Value.OnSpace,
                    });
                }
            }
            return result;
        }


        /// <summary>
        /// 检查身上时装所占用的部位
        /// </summary>
        /// <param name="avatar"></param>
        /// <returns></returns>
        private static Dictionary<int, int> GetAvatarParts(List<int> avatar)
        {
            Dictionary<int, int> parts = new Dictionary<int, int>(); //key=部位    value = 对应物品id 1对多
            for (int i = 0; i < avatar.Count; i++)
            {
                var tmp = GetForId(avatar[i]);
                if (tmp.Parts.Count > 1)    //时装对应多个部位
                {
                    foreach (var partId in tmp.Parts)
                    {
                        parts.Add(partId, tmp.Id);
                    }
                }
                else
                {
                    parts.Add(tmp.Parts[0], tmp.Id);
                }
            }
            return parts;
        }


        /// <summary>
        /// 增加金钱
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<BagInfo> GoldCtrl(int itemId, long count)
        {
            BagInfo result = new BagInfo();
            var item = GetItemById(itemId);
            if (item == null)
            {
                return null;
            }
            var money = item.Count * count;
            var bgInfo = LogicServer.User.bag;

            bgInfo.Items.TryGetValue(item.Id, out Model.Data.General.Item value);

            checked
            {
                try
                {
                    value.CurCount += money;    //更新值
                }
                catch (OverflowException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            await BagDataHelper.Instance.UpdateBagByRoleId(bgInfo, LogicServer.User.role.Id);    //保存

            result.CurUsedCell = bgInfo.CurUsedCell;
            result.MaxCellNumber = bgInfo.MaxCellNumber;
            foreach (var i in bgInfo.Items)
            {
                result.Items.Add(i.Key, new LoadRoleBagInfo()
                {
                    CurCount = i.Value.CurCount,
                    OnSpace = i.Value.OnSpace
                });
            }
            return result;
        }



        /// <summary>
        /// 多次使用同一个物品
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public async Task<BaseResponseData> UseItemsAsync(IReliableStateManager sm, Guid roleId, int itemId, long count)
        {
            UseItemResult result = new UseItemResult();
            if (itemId <= 0) throw new ArgumentNullException();
            //1 检查传过来的道具类型是否都是可以使用的类型
            if (!(CheckItemType(itemId)))
            {
                result.Result = GameEnum.WsResult.ItemTypeError;
                return result;   //道具不可使用
            }
            if (!(CheckItemInBag(itemId, count)))
            {
                result.Result = GameEnum.WsResult.NotItemInBag;
                return result;
            }
            else
            {
                var item = GetItemById(itemId); //获取物品
                if (item != null)
                { //检查玩家级别是否可以使用该物品
                    if (CheckItemLevel(itemId))
                    {
                        switch (item.Type)
                        {
                            case 1://普通物品 可以使用
                                result = await UseGeneralItem(itemId, count);
                                break;
                            case (int)Currency.Coin://金钱类物品
                            case (int)Currency.Gold://金钱类物品
                                result.BagInfo = await GoldCtrl(itemId, count);
                                FinanceLogData loginfo = new FinanceLogData()
                                {
                                    Count = item.Sell.Count * count,
                                    EventName = item.Name,
                                    MoneyType = item.Sell.CurrencyID,
                                    Type = (int)GameEnum.FinanceLog.SellItem
                                };
                                await FinanceLogController.Instance.UpdateFinanceLog(roleId, loginfo);
                                await MsgSender.Instance.FinanceLogUpdate(loginfo);
                                break;
                        }
                        return result;
                    }
                    else
                    {
                        result.Result = GameEnum.WsResult.NeedLevel;
                    }
                }
                else
                {
                    result.Result = GameEnum.WsResult.NotItemConfig;
                }
            }
            return result;
        }


        /// <summary>
        /// 检查道具类型是否是可以使用的类型
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private bool CheckItemType(int itemId)
        {
            var item = GetItemById(itemId);
            if (item == null)
            {
                return false;
            }
            if (item.Type != 1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查玩家级别是否达到使用要求
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private bool CheckItemLevel(int itemId)
        {
            var item = GetItemById(itemId); //获取物品
            if (item != null)
            {
                if (LogicServer.User.role.Level <= 0 || item.Level > LogicServer.User.role.Level)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检查单个物品在背包中是否存在
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns>true 存在 false 不存在</returns>
        private bool CheckItemInBag(int itemId, long count)
        {
            var bgInfo = LogicServer.User.bag;    //获取包中的道具
            if (!bgInfo.Items.TryGetValue(itemId, out Model.Data.General.Item item)) return false;
            if (item.CurCount < count) return false;
            return true;
        }

        /// <summary>
        /// 使用普通道具
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private async Task<UseItemResult> UseGeneralItem(int itemId, long count)
        {
            var item = GetItemById(itemId);
            if (item == null)
            {
                return null;
            }
            UseItemResult result = new UseItemResult();
            //检查背包空间
            if (!CheckBagSpace(itemId, count))
            {
                result.Result = GameEnum.WsResult.NotEnoughBagSpace;
                return result;
            }
            var itemCount = GetUsedItems(itemId, count);    //拿到将要造成的影响 道具 id  道具数量

            //删除使用的道具
            if (!(await RemoveItemsAsync(itemId, count)))
            {
                result.Result = GameEnum.WsResult.NotEnoughItem;
                return result;
            }

            //区分一下 是增加属性  还是增加道具
            return await UseItemResult(itemCount);
        }


        /// <summary>
        /// 使用道具的结果 加道具/属性/经验/金钱/打开UI
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="dicItem"></param>
        /// <returns></returns>
        public async Task<UseItemResult> UseItemResult(Dictionary<int, ItemTypeAndCount> dicItem)
        {
            if (dicItem == null) throw new ArgumentNullException();
            UseItemResult result = new UseItemResult();
            foreach (var item in dicItem)
            {
                if (item.Key <= 0) continue;
                var itemId = item.Key;
                var itemCount = item.Value.Count;   //数量
                switch (item.Value.ItemType)
                {
                    case UseEffet.GetItem://获得道具
                        await AddItemToRoleBag(itemId, itemCount);
                        break;
                    case UseEffet.AddAttribute://增加属性
                        var attr = await AddRoleAttrByUseItemAsync(itemId, itemCount);
                        result.ChangeAttr.Add(new Model.ResponseData.UserAttr()
                        {
                            Count = attr.Count,
                            UserAttrID = attr.UserAttrID
                        });
                        break;
                    case UseEffet.AddCurrency:  //增加金钱
                        await AddMoneyByUseItemAsync(itemId, itemCount);
                        break;
                }
            }
            foreach (var b in LogicServer.User.bag.Items)
            {
                result.BagInfo.Items.Add(b.Key, new LoadRoleBagInfo()
                {
                    CurCount = b.Value.CurCount,
                    OnSpace = b.Value.OnSpace
                });
            }
            result.BagInfo.CurUsedCell = LogicServer.User.bag.CurUsedCell;
            result.BagInfo.MaxCellNumber = LogicServer.User.bag.MaxCellNumber;


            return result;
        }



        /// <summary>
        /// 使用物品后增加金钱
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="moneyId"></param>
        /// <param name="moneyCount"></param>
        /// <returns></returns>
        private async Task<LoadRoleBagInfo> AddMoneyByUseItemAsync(int moneyId, long moneyCount)
        {
            LoadRoleBagInfo result = new LoadRoleBagInfo();
            var roleBg = LogicServer.User.bag;
            var roleId = LogicServer.User.role.Id;

            if (roleBg.Items.TryGetValue(moneyId, out Model.Data.General.Item value))
            {
                checked
                {
                    try
                    {
                        value.CurCount += moneyCount;
                    }
                    catch (OverflowException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            await BagDataHelper.Instance.UpdateBagByRoleId(roleBg, LogicServer.User.role.Id);   //保存
            FinanceLogData loginfo = new FinanceLogData()
            {
                Count = moneyCount,
                MoneyType = moneyId,
                Type = (int )GameEnum.FinanceLog.SellItem
            };
            await FinanceLogController.Instance.UpdateFinanceLog(roleId, loginfo);
            await MsgSender.Instance.FinanceLogUpdate(loginfo);
            result.CurCount = value.CurCount;
            result.OnSpace = 0;
            return result;
        }


        /// <summary>
        /// 使用道具后增加用户属性
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private async Task<Model.Data.Npc.UserAttr> AddRoleAttrByUseItemAsync(int itemId, long count)
        {

            var role = LogicServer.User.role;
            Model.Data.Npc.UserAttr attr = new Model.Data.Npc.UserAttr();
            var item = GetItemById(itemId);
            if (item == null)
            {
                return null;
            }
            foreach (var j in item.Attribute)
            {
                var tmp = role.UserAttr.FirstOrDefault(p => p.UserAttrID == j.AttributeID);
                if (tmp != null)
                {
                    checked
                    {
                        try
                        {
                            tmp.Count += j.Count;
                        }
                        catch (OverflowException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                    attr = tmp;
                }
            }
            await RoleDataHelper.Instance.UpdateRoleByRoleIdAsync(role.Id, role);

            return attr;
        }



        /// <summary>
        /// 添加道具进背包
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<AddItemResult> AddItemToRoleBag(int itemId, long count)
        {
            var item = GetItemById(itemId);
            if (item == null)
            {
                return null;
            }
            AddItemResult result = new AddItemResult();
            if (!CheckBagSpace(itemId, count))
            {
                result.Result = GameEnum.WsResult.NotEnoughBagSpace;
                return result;
            }
            var itemCount = new Dictionary<int, long>();
            itemCount.Add(itemId, count);
            result = await AddItemToRoleBag(itemCount);

            var data = await InitHelpers.GetPse().SerializeAsync(result);
            await MsgMaker.SendMessage(WSResponseMsgID.AddItemResult, 1, data);    //写入队列
            return result;
        }


        /// <summary>
        /// 添加道具进用户背包
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemCount"></param>
        /// <returns></returns>
        private async Task<AddItemResult> AddItemToRoleBag(Dictionary<int, long> itemCount)
        {
            AddItemResult result = new AddItemResult();
            var roleInfo = LogicServer.User.role;
            var roleBg = LogicServer.User.bag;
            long oldSocialStatus = 0;
            foreach (var i in roleBg.Items)
            {
                var item = GetItemById(i.Key);
                if (item != null)
                {
                    checked
                    {
                        try
                        {
                            if (item.Superposition == 0)  //身价值不叠加
                            {

                                oldSocialStatus += item.Status;
                            }
                            else if (item.Superposition == 1)  //叠加
                            {
                                oldSocialStatus += (item.Status * i.Value.CurCount);
                            }
                        }
                        catch (OverflowException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }
            long socialStatus = roleInfo.SocialStatus - oldSocialStatus;    //除去背包中所有物品带来的身价值

            var curBagCell = roleBg.CurUsedCell;    //当前背包所用格子
            foreach (var b in itemCount)
            {
                var itemTemplate = GetItemById(b.Key);
                checked
                {
                    try
                    {
                        if (roleBg.Items.TryGetValue(b.Key, out Model.Data.General.Item value))    //存在
                        {

                            curBagCell -= value.OnSpace;  //去除原先占用的格子
                            if (itemTemplate.Count == 1)
                            {


                                value.OnSpace = value.CurCount += b.Value;  //当前数量和空间
                            }
                            else
                            {

                                value.CurCount += b.Value;    //当前数量
                                value.OnSpace = (value.CurCount / itemTemplate.Count) + 1;
                            }
                            curBagCell += value.OnSpace;  //更新加入物品后的格子
                        }
                        else
                        {
                            Model.Data.General.Item i = new Model.Data.General.Item();
                            if (itemTemplate.Count == 1)
                            {

                                i.OnSpace = i.CurCount = b.Value;   //当前数量和空间
                            }
                            else
                            {

                                i.CurCount += b.Value;
                                i.OnSpace = (i.CurCount / itemTemplate.Count) + 1;
                            }
                            curBagCell += i.OnSpace;  //更新加入物品后的格子
                            roleBg.Items.Add(b.Key, i);
                        }
                    }
                    catch (OverflowException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }

            }
            roleBg.CurUsedCell = curBagCell;

            await BagDataHelper.Instance.UpdateBagByRoleId(roleBg, LogicServer.User.role.Id);

            foreach (var status in roleBg.Items)
            {
                var item = GetItemById(status.Key);
                if (item != null)
                {
                    checked
                    {
                        try
                        {
                            if (item.Superposition == 0)
                            {
                                socialStatus += item.Status;
                            }
                            else
                            {
                                socialStatus += (item.Status * status.Value.CurCount);
                            }
                        }
                        catch (OverflowException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                    }
                }
            }

            roleInfo.SocialStatus = socialStatus;   //身价值更新
            await RoleDataHelper.Instance.UpdateRoleByRoleIdAsync(roleInfo.Id, roleInfo); //保存



            result.ShenJia = socialStatus;
            result.BagInfo.CurUsedCell = roleBg.CurUsedCell;
            foreach (var i in roleBg.Items)
            {
                result.BagInfo.Items.Add(i.Key, new LoadRoleBagInfo()
                {
                    OnSpace = i.Value.OnSpace,
                    CurCount = i.Value.CurCount,
                });
            }
            return result;
        }

        /// <summary>
        /// 检查背包空间
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="items">增加的道具</param>
        /// <returns>true 空间足够  false 空间不够</returns>
        private bool CheckBagSpace(int itemId, long count)
        {
            var roleBg = LogicServer.User.bag;
            if (roleBg.CurUsedCell >= roleBg.MaxCellNumber)
            {
                return false;
            }
            Int64 newOnSpace = 0;
            var item = GetItemById(itemId);
            if (item == null)
            {
                return false;
            }
            if (roleBg.Items.TryGetValue(itemId, out Model.Data.General.Item value))
            {//有同类物品
                if (item.Count == 1)    //不可叠加
                {
                    newOnSpace += count;
                }
                else
                {   //可叠加
                    var oldSpace = value.OnSpace;    //旧的占用空间
                    var curSpace = (value.CurCount + count) / item.Count;   //物品增加后占用空间
                    if (curSpace > oldSpace)
                    {
                        newOnSpace += (curSpace - oldSpace);
                    }
                }
            }
            else
            {
                if (item.Count == 1)
                {
                    newOnSpace += count;
                }
                else
                {
                    newOnSpace = count / item.Count;   //物品增加后占用空间
                }
            }
            if ((newOnSpace + roleBg.CurUsedCell) > roleBg.MaxCellNumber)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 获得使用道具后得到属性/物品/经验/金钱/打开ui 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>

        private Dictionary<int, ItemTypeAndCount> GetUsedItems(int itemId, long count)
        {
            var item = GetItemById(itemId);
            if (item == null)
            {
                return null;
            }
            Dictionary<int, ItemTypeAndCount> dic = new Dictionary<int, ItemTypeAndCount>();
            foreach (var effet in item.UseEffet)
            {
                ItemTypeAndCount typeCount = new ItemTypeAndCount();
                typeCount.ItemType = UseEffet.GetItem;
                checked
                {
                    try
                    {
                        typeCount.Count = effet.Value2 * count;
                    }
                    catch (OverflowException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                dic.Add(effet.Value1, typeCount);
            }
            return dic;
        }




        /// <summary>
        /// 检查背包是否已达到空间上限
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns>true达到上限  false未达到上限</returns>
        private bool CheckItemSpace(IReliableStateManager sm, Guid roleId)
        {
            if (LogicServer.User.bag.CurUsedCell >= LogicServer.User.bag.MaxCellNumber)
                return true;
            return false;
        }



        /// <summary>
        /// 出售商品
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>

        public async Task<SellItemResult> SellItemsAsync(int itemId, long count)
        {
            try
            {
                SellItemResult result = new SellItemResult();
                var bgInfo = LogicServer.User.bag;
                var roleId = LogicServer.User.role.Id;
                var item = GetItemById(itemId);
                if (item == null) { result.Result = GameEnum.WsResult.NotItemConfig; return result; }

                var money = item.Sell;

                if (!await RemoveItemsAsync(itemId, count)) { result.Result = GameEnum.WsResult.RemoveItemErr; return result; }

                long financeMoney = 0;
                bgInfo.Items.TryGetValue(money.CurrencyID, out Model.Data.General.Item itemMoney);
                checked
                {
                    try
                    {
                        financeMoney += (money.Count * count);
                        itemMoney.CurCount += (money.Count * count);
                    }
                    catch (OverflowException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                await BagDataHelper.Instance.UpdateBagByRoleId(bgInfo, LogicServer.User.role.Id);

                await MsgSender.Instance.GoldUpdate(money.CurrencyID);

                FinanceLogData loginfo = new FinanceLogData()
                {
                    Count = financeMoney,
                    EventName = item.Name,
                    MoneyType = money.CurrencyID,
                    Type = (int)GameEnum.FinanceLog.SellItem
                };
                await FinanceLogController.Instance.UpdateFinanceLog(roleId, loginfo);  //财务日志变动消息
                await MsgSender.Instance.FinanceLogUpdate(loginfo);
                var retBg = LogicServer.User.bag;
                var retRoInfo = LogicServer.User.role;
                result.BagInfo.CurUsedCell = retBg.CurUsedCell;
                result.BagInfo.MaxCellNumber = retBg.MaxCellNumber;
                foreach (var i in retBg.Items)
                {
                    result.BagInfo.Items.Add(i.Key, new LoadRoleBagInfo()
                    {
                        CurCount = i.Value.CurCount,
                        OnSpace = i.Value.OnSpace
                    });
                }
                result.ShenJia = retRoInfo.SocialStatus;
                return result;
            }
            catch (Exception ex)
            {
                //日志
                throw ex;
            }

        }


        /// <summary>
        /// 移除背包中的道具
        /// </summary>s
        /// <param name="sm"></param>
        /// <param name="item"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<bool> RemoveItemsAsync(int itemId, long count)
        {
            try
            {
                if (count <= 0)
                {
                    return false;
                }
                var item = GetItemById(itemId);
                if (item == null)
                    return false;
                var bg = LogicServer.User.bag;
                var role = LogicServer.User.role;
                long shenjia = 0;

                if (!bg.Items.TryGetValue(itemId, out Model.Data.General.Item i))
                {
                    return false;
                }
                else
                {
                    //减少数量  减少到0的时候  删除 暂时无此需求
                    if (i.CurCount > count)
                    {
                        if (item.Superposition == 0)
                        {
                            role.SocialStatus -= item.Status;   //减少身价值
                            shenjia = -item.Status;
                        }
                        else
                        {
                            role.SocialStatus -= (item.Status * count);
                            shenjia = -(item.Status * count);
                        }

                        var newCount = i.CurCount - count;
                        i.CurCount = newCount;         //重新计算数量
                    }
                    else if (i.CurCount == count)
                    {
                        bg.Items.Remove(itemId);
                        if (item.Superposition == 0)
                        {
                            role.SocialStatus -= item.Status;   //减少身价值
                            shenjia = -item.Status;
                        }
                        else
                        {
                            role.SocialStatus -= (item.Status * count);
                            shenjia = -(item.Status * count);
                        }
                    }
                    else
                    {
                        return false;   //数量不够 无法删除
                    }

                    using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                    {
                        await BagDataHelper.Instance.UpdateBagByRoleId(bg, LogicServer.User.role.Id, tx);//更新背包信息
                        await RoleDataHelper.Instance.UpdateRoleByRoleIdAsync(role.Id, role, tx);
                        await RoleController.Instance.AddIncome(role.Id, shenjia, tx);
                        await tx.CommitAsync();
                    }
                    await MsgSender.Instance.ItemUpdate(itemId, i.CurCount);  //道具变动消息
                    await MsgSender.Instance.UpdateIncome(); //身价变动消息
                    return true;
                }
            }
            catch (Exception ex)
            {
                //TODO日志
                throw ex;
            }
        }


        public AutoData.Item GetItemById(int id)
        {
            return AutoData.Item.GetForId(id);
        }
    }
}
