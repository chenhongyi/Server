using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class RoleAttrListDataHelper : DataHelperBase<RoleAttrListDataHelper, List<Model.Data.Npc.UserAttr>>
    {
        public async Task<List<Model.Data.Npc.UserAttr>> GetRoleAttrByRoleId(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var roleAttr = await db.TryGetValueAsync(tx, roleId);
                return roleAttr.HasValue ? roleAttr.Value : null;
            }
        }

        public async Task SetRoleAttrByRoleId(Guid roleId,List<Model.Data.Npc.UserAttr> roleAttr)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleId, roleAttr);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateRoleAttrByRoleId(Guid roleId,List<Model.Data.Npc.UserAttr> roleAttr)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, roleId, roleAttr);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveRoleAttrByRoleId(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, roleId);
                await tx.CommitAsync();
            }
        }

    }
}
