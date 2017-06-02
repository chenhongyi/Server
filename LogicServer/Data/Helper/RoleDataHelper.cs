using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Data.Npc;
using Microsoft.ServiceFabric.Data;

namespace LogicServer.Data.Helper
{
    public class RoleDataHelper : DataHelperBase<RoleDataHelper, Model.Data.Npc.UserRole>
    {
        public async Task<Model.Data.Npc.UserRole> GetRoleByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var role = await db.TryGetValueAsync(tx, roleId);
                return role.HasValue ? role.Value : null;
            }
        }

        public async Task SetRoleByRoleIdAsync(Guid roleId, Model.Data.Npc.UserRole role)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleId, role);
                await tx.CommitAsync();
            }
        }


        public async Task UpdateRoleByRoleIdAsync(Guid roleId, Model.Data.Npc.UserRole role)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, roleId, role);
                await tx.CommitAsync();
            }
        }


        public async Task RemoveRoleByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, roleId);
                await tx.CommitAsync();
            }
        }


        public async Task SetRoleByRoleIdAsync(Guid roleId, Model.Data.Npc.UserRole role, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.AddAsync(tx, roleId, role);
        }


        public async Task UpdateRoleByRoleIdAsync(Guid roleId, Model.Data.Npc.UserRole role, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.SetAsync(tx, roleId, role);
        }


        public async Task RemoveRoleByRoleIdAsync(Guid roleId, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.TryRemoveAsync(tx, roleId);
        }

        public async Task<UserRole> GetRoleBySidAsync(string sid)
        {
            var db = await GetString(LogicServer.Instance.StateManager);
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var role = await db.TryGetValueAsync(tx, sid);
                return role.HasValue ? role.Value : null;
            }
        }

        public async Task SetRoleBySidAsync(string sid, UserRole user)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, sid, user);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateRoleBySidAsync(string sid, UserRole user)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, sid, user);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveRoleBySidAsync(string sid)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, sid);
                await tx.CommitAsync();
            }
        }

        public async Task SetRoleBySidAsync(string sid, UserRole user, ITransaction tx)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            await db.AddAsync(tx, sid, user);
        }

        public async Task UpdateRoleBySidAsync(string sid, UserRole user, ITransaction tx)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            await db.SetAsync(tx, sid, user);
        }

        public async Task RemoveRoleBySidAsync(string sid, ITransaction tx)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            await db.TryRemoveAsync(tx, sid);
        }
    }
}
