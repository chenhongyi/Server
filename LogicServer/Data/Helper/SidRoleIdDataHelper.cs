using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class SidRoleIdDataHelper : DataHelperBase<SidRoleIdDataHelper, string>
    {
        #region RoleId操作

        public async Task<string> GetSidByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var sid = await db.TryGetValueAsync(tx, roleId);
                return sid.HasValue ? sid.Value : null;
            }
        }

        public async Task SetSidByRoleIdAsync(Guid roleId,string sid)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleId, sid);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateSidByRoleIdAsync(Guid roleId, string sid)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, roleId, sid);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveSidByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, roleId);
                await tx.CommitAsync();
            }
        }
        #endregion


        public async Task<string> GetRoleIdBySidAsync(string sid)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var roleid = await db.TryGetValueAsync(tx, sid);
                return roleid.HasValue ? roleid.Value.ToString() : null;
            }
        }

        public async Task SetRoleIdBySidAsync(string sid, Guid roleId)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, sid, roleId.ToString());
                await tx.CommitAsync();
            }
        }

        public async Task UpdateRoleIdBySidAsync(string sid, Guid roleId)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, sid, roleId.ToString());
                await tx.CommitAsync();
            }
        }

        public async Task RemoveRoleIdBySidAsync(string sid)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, sid);
                await tx.CommitAsync();
            }
        }
    }
}
