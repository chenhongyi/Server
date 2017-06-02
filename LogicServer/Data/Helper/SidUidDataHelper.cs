using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    /// <summary>
    /// session与username之间的相互操作类
    /// </summary>
    public class SidUidDataHelper : DataHelperBase<SidUidDataHelper, string>
    {
        #region session操作

        /// <summary>
        /// 获取账号
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public async Task<string> GetUserNameBySidAsync(string sid)
        {
            try
            {
                var db = await GetString(LogicServer.Instance.StateManager);
                using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                {
                    var userName = await db.TryGetValueAsync(tx, sid);
                    return userName.HasValue ? userName.Value : null;
                }
            }

            catch (Exception ex)
            {
                //Log
                var log = ex.Message;
                throw ex;
            }
        }

        public async Task SetUserNameBySidAsync(string sid, string userName)
        {
            try
            {
                var db =await GetString(LogicServer.Instance.StateManager);
                using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                {
                    await db.AddAsync(tx, sid, userName);
                    await tx.CommitAsync();
                }
            }

            catch (Exception ex)
            {
                //Log
                var log = ex.Message;
            }
        }


        public async Task UpdateUserNameBySidAsync(string sid, string userName)
        {
            try
            {
                var db =await GetString(LogicServer.Instance.StateManager);
                using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                {
                    await db.SetAsync(tx, sid, userName);
                    await tx.CommitAsync();
                }
            }

            catch (Exception ex)
            {
                //Log
                var log = ex.Message;
            }
        }

        public async Task RemoveUserNameBySidAsync(string sid)
        {
            try
            {
                var db = await GetString(LogicServer.Instance.StateManager);
                using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                {
                    await db.TryRemoveAsync(tx, sid);
                    await tx.CommitAsync();
                }
            }

            catch (Exception ex)
            {
                //Log
                var log = ex.Message;
            }
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////

        #region userName操作
        public async Task<string> GetSidByUserNameAsync(string userName)
        {
            try
            {
                var db = await GetString(LogicServer.Instance.StateManager);
                using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                {
                    var sid = await db.TryGetValueAsync(tx, userName);
                    return sid.HasValue ? sid.Value : null;
                }
            }

            catch (Exception ex)
            {
                //Log
                var log = ex.Message;
                throw ex;
            }
        }

        public async Task SetSidByUserNameAsync(string userName, string sid)
        {
            try
            {
                var db = await GetString(LogicServer.Instance.StateManager);
                using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                {
                    await db.AddAsync(tx, userName, sid);
                    await tx.CommitAsync();
                }
            }

            catch (Exception ex)
            {
                //Log
                var log = ex.Message;
            }
        }

        public async Task UpdateSidByUserNameAsync(string userName, string sid)
        {
            try
            {
                var db = await GetString(LogicServer.Instance.StateManager);
                using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                {
                    await db.SetAsync(tx, userName, sid);
                    await tx.CommitAsync();
                }
            }

            catch (Exception ex)
            {
                //Log
                var log = ex.Message;
            }
        }

        public async Task RemoveSidByUserNameAsync(string userName)
        {
            try
            {
                var db =await GetString(LogicServer.Instance.StateManager);
                using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                {
                    await db.TryRemoveAsync(tx, userName);
                    await tx.CommitAsync();
                }
            }

            catch (Exception ex)
            {
                //Log
                var log = ex.Message;
            }
        }
        #endregion


    }
}
