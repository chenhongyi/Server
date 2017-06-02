using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class FinanceLogDataHelper : DataHelperBase<FinanceLogDataHelper, Model.Data.Business.FinanceLogData>
    {

        public async Task<Model.Data.Business.FinanceLogData> GetFinLogByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var log = await db.TryGetValueAsync(tx, roleId);
                return log.HasValue ? log.Value : null;
            }
        }


        public async Task SetFinLogByRoleIdAsync(Guid roleId,Model.Data.Business.FinanceLogData data)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleId, data);
                await tx.CommitAsync();
            }
        }


        public async Task UpdateFinLogByRoleIdAsync(Guid roleId,Model.Data.Business.FinanceLogData data)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, roleId, data);
                await tx.CommitAsync();
            }
        }


        public async Task RemoveFinLogByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx,roleId);
                await tx.CommitAsync();
            }
        }

    }

    public class FinanceLogListDataHelper : DataHelperBase<FinanceLogListDataHelper, List<Model.Data.Business.FinanceLogData>>
    {
        public async Task<List<Model.Data.Business.FinanceLogData>> GetFinLogListByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var list = await db.TryGetValueAsync(tx, roleId);
                return list.HasValue ? list.Value : null;
            }
        }

        public async Task SetFinLogListByRoleIdAsync(Guid roleId,List<Model.Data.Business.FinanceLogData> list)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleId, list);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateFinLogListByRoleIdAsync(Guid roleId,List<Model.Data.Business.FinanceLogData> list)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx,roleId, list);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveFinLogListByRoleIdAsync(Guid roleId)
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
