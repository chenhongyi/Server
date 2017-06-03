using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class RoomData : DataHelperBase<RoomData, Model.Data.Npc.Room>
    {
        /// <summary>
        /// 通过角色id获取角色背包信息
        /// </summary>
        public async Task<Model.Data.Npc.Room> GetRoomData()
        {
            var sm = LogicServer.Instance.StateManager;
            var db = await GetGuid(sm);
            using (var tx = sm.CreateTransaction())
            {
                var box = await db.TryGetValueAsync(tx, LogicServer.User.role.Id);
                return box.HasValue ? box.Value : null;
            }
        }
        /// <summary>
        /// 更新角色房间信息
        /// </summary>
        public async Task UpdateRoomData(Model.Data.Npc.Room data)
        {
            var sm = LogicServer.Instance.StateManager;
            var db = GetGuid(sm).Result;
            using (var tx = sm.CreateTransaction())
            {
                await db.SetAsync(tx, LogicServer.User.role.Id, data);
                await tx.CommitAsync();
            }
        }
    }
}