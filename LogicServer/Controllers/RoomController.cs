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
        /// <summary>
        /// 获得房间信息
        /// </summary>
        public async Task<BaseResponseData> GetRoom()
        {
            return await GetRoom(LogicServer.User.role.Id);
        }
        public async Task<BaseResponseData> GetRoom(Guid Id)
        {
            RoomResult result = new RoomResult();
            var data = await Data.Helper.RoomData.Instance.GetRoomData(LogicServer.Instance.StateManager, Id);
            if (data == null)
            {
                //初始化
                data = new Room();
                data.Id = Id.ToString();
                data.RoomId = 1;
                data.Config = new Dictionary<int, int>();

                var room = AutoData.Room.GetForKey(data.RoomId);
                foreach (var fur in room.Furniture)
                {
                    data.Config[fur.Point] = fur.ItemID;
                }

                await Data.Helper.RoomData.Instance.UpdateRoomData(LogicServer.Instance.StateManager, Id, data);
            }

            result.Id = data.Id;
            result.RoomId = data.RoomId;
            result.Config = new List<RoomFurConfig>();
            foreach (var kv in data.Config)
            {
                result.Config.Add(new RoomFurConfig(kv.Key, kv.Value));
            }
            return result;
        }
    }
}