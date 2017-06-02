using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Data.Account;

namespace LogicServer.Data.Helper
{
    public class LoginDataHelper : DataHelperBase<LoginDataHelper, Model.Data.Account.Login>
    {

        #region     AccountId 与 Login 互操作
        public async Task<Model.Data.Account.Login> GetLoginByAccountId(Guid accountId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var user = await db.TryGetValueAsync(tx, accountId);
                return user.HasValue ? user.Value : null;
            }
        }

        public async Task SetLoginByAccountId(Guid accountId, Model.Data.Account.Login login)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, accountId, login);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateLoginByAccountId(Guid accountId, Model.Data.Account.Login login)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, accountId, login);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveLoginByAccountId(Guid accountId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, accountId);
                await tx.CommitAsync();
            }
        }
        #endregion

        #region UserName 与 Login 互操作


        public async Task<Model.Data.Account.Login> GetLoginByUserName(string userName)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var user = await db.TryGetValueAsync(tx, userName);
                return user.HasValue ? user.Value : null;
            }
        }

        public async Task SetLoginByUserName(string userName, Model.Data.Account.Login login)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, userName, login);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateLoginByUserName(string userName, Model.Data.Account.Login login)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, userName, login);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveLoginByUserName(string userName)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, userName);
                await tx.CommitAsync();
            }
        }
        #endregion

        #region IMEI 与 Login 互操作


        public async Task<Model.Data.Account.Login> GetLoginByIMEI(string imei)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var login = await db.TryGetValueAsync(tx, imei);
                return login.HasValue ? login.Value : null;
            }
        }

        public async Task SetLoginByIMEI(string imei, Login login)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, imei, login);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateLoginByIMEI(string imei, Login login)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, imei, login);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveLoginByIMEI(string imei)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, imei);
                await tx.CommitAsync();
            }
        }
        #endregion
    }
}
