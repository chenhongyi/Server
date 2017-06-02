using Microsoft.ServiceFabric.Data;
using Model.Data.Account;
using System;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class AccountDataHelper : DataHelperBase<AccountDataHelper, Account>
    {



        #region roleId 与 Account 互操作


        public async Task<Model.Data.Account.Account> GetAccountByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var user = await db.TryGetValueAsync(tx, roleId);
                return user.HasValue ? user.Value : null;
            }
        }

        public async Task SetAccountByRoleIdAsync(Guid roleId, Account account)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleId, account);
                await tx.CommitAsync();
            }
        }
        public async Task UpdateAccountByRoleIdAsync(Guid roleId, Account account)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, roleId, account);
                await tx.CommitAsync();
            }
        }
        public async Task RemoveAccountByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, roleId);
                await tx.CommitAsync();
            }
        }
        #endregion



        #region UserName 与 Account 互操作
        public async Task<Model.Data.Account.Account> GetAccountByUserNameAsync(string userName)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var account = await db.TryGetValueAsync(tx, userName);
                return account.HasValue ? account.Value : null;
            }
        }

        public async Task SetAccountByUserNameAsync(string userName, Account account)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, userName, account);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateAccountByUserNameAsync(string userName, Account account)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, userName, account);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveAccountByUserNameAsync(string userName)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, userName);
                await tx.CommitAsync();
            }
        }
        #endregion

        #region AccountId 与 Account 互操作
        public async Task<Model.Data.Account.Account> GetAccountByAccountIdAsync(Guid accountId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var user = await db.TryGetValueAsync(tx, accountId);
                return user.HasValue ? user.Value : null;
            }
        }

        public async Task SetAccountByAccountIdAsync(Guid accountId, Account account)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, accountId, account);
                await tx.CommitAsync();
            }
        }
        public async Task UpdateAccountByAccountIdAsync(Guid accountId, Account account)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, accountId, account);
                await tx.CommitAsync();
            }
        }
        public async Task RemoveAccountByAccountIdAsync(Guid accountId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, accountId);
                await tx.CommitAsync();
            }
        }

        #endregion

        public async Task<Account> GetAccountBySidAsync(string session)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var account = await db.TryGetValueAsync(tx, session);
                return account.HasValue ? account.Value : null;
            }
        }

        public async Task SetAccountBySidAsync(string session, Account account)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, session, account);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateAccountBySidAsync(string session, Account account)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, session, account);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveAccountBySidAsync(string session)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, session);
                await tx.CommitAsync();
            }
        }


    }
}
