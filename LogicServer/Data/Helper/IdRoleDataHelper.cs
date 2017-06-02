using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class IdRoleDataHelper : DataHelperBase<IdRoleDataHelper, Model.Data.Npc.IdRole>
    {


        public async Task<bool> CheckIdRoleByRoleNameAsync(string roleName)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                return await db.ContainsKeyAsync(tx, roleName);
            }
        }


        public async Task<Model.Data.Npc.IdRole> GetIdRoleByRoleNameAsync(string roleName)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var idRole = await db.TryGetValueAsync(tx, roleName);
                return idRole.HasValue ? idRole.Value : null;
            }
        }

        public async Task SetIdRoleByRoleNameAsync(string roleName, Model.Data.Npc.IdRole idRole)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleName, idRole);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateIdRoleByRoleNameAsync(string roleName, Model.Data.Npc.IdRole idRole)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, roleName, idRole);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveIdRoleByRoleNameAsync(string roleName)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, roleName);
                await tx.CommitAsync();
            }
        }
    }
}
