using LogicServer.Data.Helper;
using Microsoft.ServiceFabric.Data;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Controllers
{
    public class RoleController : BaseInstance<RoleController>
    {
        public async Task AddIncome(Guid roleId, long income)
        {
            var role = LogicServer.User.role;
            role.SocialStatus += income;
            await RoleDataHelper.Instance.UpdateRoleByRoleIdAsync(roleId, role);
        }


        public async Task AddIncome(Guid roleId, long income, ITransaction tx)
        {
            var role = LogicServer.User.role;
            role.SocialStatus += income;
            await RoleDataHelper.Instance.UpdateRoleByRoleIdAsync(roleId, role, tx);
        }

        public async Task DecIncome(Guid roleId, long income)
        {
            var role = LogicServer.User.role;
            role.SocialStatus += income;
            await RoleDataHelper.Instance.UpdateRoleByRoleIdAsync(roleId, role);
        }


        public async Task DecIncome(Guid roleId, long income, ITransaction tx)
        {
            var role = LogicServer.User.role;
            role.SocialStatus += income;
            await RoleDataHelper.Instance.UpdateRoleByRoleIdAsync(roleId, role, tx);
        }
    }
}
