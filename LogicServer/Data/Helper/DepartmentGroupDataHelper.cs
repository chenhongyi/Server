using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class DepartmentGroupDataHelper : DataHelperBase<DepartmentGroupDataHelper, Model.Data.Business.DepartmentGroup>
    {
        public async Task<Model.Data.Business.DepartmentGroup> GetDepartMentGroupByRoleId(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var group = await db.TryGetValueAsync(tx, roleId);
                return group.HasValue ? group.Value : null;
            }
        }
        public async Task SetDepartMentGroupByRoleId(Guid roleId, Model.Data.Business.DepartmentGroup group)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleId, group);
                await tx.CommitAsync();
            }
        }
        public async Task UpdateDepartMentGroupByRoleId(Guid roleId, Model.Data.Business.DepartmentGroup group)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, roleId, group);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveDepartMentGroupByRoleId(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, roleId);
                await tx.CommitAsync();
            }
        }


        public async Task SetDepartMentGroupByRoleId(Guid roleId, Model.Data.Business.DepartmentGroup group, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.AddAsync(tx, roleId, group);
        }
        public async Task UpdateDepartMentGroupByRoleId(Guid roleId, Model.Data.Business.DepartmentGroup group, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.SetAsync(tx, roleId, group);
        }

        public async Task RemoveDepartMentGroupByRoleId(Guid roleId, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.TryRemoveAsync(tx, roleId);
        }

    }
}
