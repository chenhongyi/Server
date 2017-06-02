using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class PosListDataHelper : DataHelperBase<PosListDataHelper, List<int>>
    {
        public async Task<List<int>> GetPosListByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var posList = await db.TryGetValueAsync(tx, roleId);
                return posList.HasValue ? posList.Value : null;
            }
        }

        public async Task SetPosListByRoleIdAsync(Guid roleId, List<int> pos)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleId, pos);
                await tx.CommitAsync();
            }
        }

        public async Task UpdatePosListByRoleIdAsync(Guid roleId, List<int> pos)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, roleId, pos);
                await tx.CommitAsync();
            }
        }


        public async Task RemovePosListByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, roleId);
                await tx.CommitAsync();
            }
        }

        public async Task SetPosListByRoleIdAsync(Guid roleId, List<int> pos, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.AddAsync(tx, roleId, pos);
        }

        public async Task UpdatePosListByRoleIdAsync(Guid roleId, List<int> pos, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.SetAsync(tx, roleId, pos);
        }


        public async Task RemovePosListByRoleIdAsync(Guid roleId, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.TryRemoveAsync(tx, roleId);
        }
    }
}
