using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class RoleListDataHelper : DataHelperBase<RoleListDataHelper, List<Model.Data.Npc.UserRole>>
    {
        public async Task<List<Model.Data.Npc.UserRole>> GetRoleListByAccountIdAsync(Guid accountId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var accountList = await db.TryGetValueAsync(tx, accountId);
                return accountList.HasValue ? accountList.Value : null;
            }
        }

        public async Task SetRoleListByAccountIdAsync(Guid accountId ,List<Model.Data.Npc.UserRole> roles)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, accountId, roles);
                await tx.CommitAsync();
            }

        }

        public async Task UpdateRoleListByAccountIdAsync(Guid accountId, List<Model.Data.Npc.UserRole> roles)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, accountId, roles);
                await tx.CommitAsync();
            }

        }



        public async Task RemoveRoleListByAccountIdAsync(Guid accountId, List<Model.Data.Npc.UserRole> roles)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, accountId);
                await tx.CommitAsync();
            }

        }
    }
}
