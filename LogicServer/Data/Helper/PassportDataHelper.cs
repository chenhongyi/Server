using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class PassportDataHelper : DataHelperBase<PassportDataHelper, Model.Data.Account.Passport>
    {
        #region imei 与 Passport 互操作



        public async Task<Model.Data.Account.Passport> GetPassportByIMEI(string imei)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var passport = await db.TryRemoveAsync(tx, imei);
                return passport.HasValue ? passport.Value : null;
            }
        }


        public async Task SetPassportByIMEI(string imei, Model.Data.Account.Passport passport)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, imei, passport);
                await tx.CommitAsync();
            }
        }
        public async Task UpdatePassportByIMEI(string imei, Model.Data.Account.Passport passport)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, imei, passport);
                await tx.CommitAsync();
            }
        }

        public async Task RemovePassportByIMEI(string imei)
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
