using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class CompanyDataHelper : DataHelperBase<CompanyDataHelper, Model.Data.Business.Company>
    {
        public async Task<bool> CheckCompanyByRoleId(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                return await db.ContainsKeyAsync(tx, roleId);
            }
        }

        public async Task<Model.Data.Business.Company> GetCompanyByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var company = await db.TryGetValueAsync(tx, roleId);
                return company.HasValue ? company.Value : null;
            }
        }

        public async Task SetCompanyByRoleIdAsync(Guid roleId, Model.Data.Business.Company company)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleId, company);
                await tx.CommitAsync();
            }
        }


        public async Task UpdateCompanyByRoleIdAsync(Guid roleId, Model.Data.Business.Company company)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, roleId, company);
                await tx.CommitAsync();
            }
        }


        public async Task RemoveCompanyByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, roleId);
                await tx.CommitAsync();
            }
        }

        public async Task SetCompanyByRoleIdAsync(Guid roleId, Model.Data.Business.Company company, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.AddAsync(tx, roleId, company);
        }


        public async Task UpdateCompanyByRoleIdAsync(Guid roleId, Model.Data.Business.Company company, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.SetAsync(tx, roleId, company);
        }


        public async Task RemoveCompanyByRoleIdAsync(Guid roleId, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.TryRemoveAsync(tx, roleId);
        }
    }

    public class CompanyNameDataHelper : DataHelperBase<CompanyNameDataHelper, string>
    {

        public async Task<bool> CheckCompanyIdByName(string name)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                return await db.ContainsKeyAsync(tx, name);
            }
        }

        public async Task<string> GetCompanyIdByName(string name)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var id = await db.TryGetValueAsync(tx, name);
                return id.HasValue ? id.Value : null;
            }
        }

        public async Task SetCompanyIdByName(string name, string id)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, name, id);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateCompanyIdByName(string name, string id)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, name, id);
                await tx.CommitAsync();
            }
        }


        public async Task RemoveCompanyIdByName(string name)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, name);
                await tx.CommitAsync();
            }
        }
    }
}
