using LogicServer.Data;
using LogicServer.Data.Helper;
using Microsoft.ServiceFabric.Data;
using Model.Data.Business;
using Model.ResponseData;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Controllers
{
    public class FinanceLogController : BaseInstance<FinanceLogController>
    {


        public async Task<List<LoadFinanceLogInfo>> GetFinanceLog(Guid roleId)
        {
            List<LoadFinanceLogInfo> result = new List<LoadFinanceLogInfo>();

            var ret = await LoadFinanceLog(roleId);
            if (ret != null)
            {
                foreach (var i in ret.FinanceLog)
                {
                    result.Add(new LoadFinanceLogInfo()
                    {
                        Count = i.Count,
                        EventName = i.EventName,
                        MoneyType = i.MoneyType,
                        Time = i.Time.ToString(),
                        Type = i.Type
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// 获取7天财务信息日志
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private async Task<LoadFinanceLogResult> LoadFinanceLog(Guid roleId)
        {

            LoadFinanceLogResult result = new LoadFinanceLogResult();
            var log = await FinanceLogListDataHelper.Instance.GetFinLogListByRoleIdAsync(roleId);
            if (log == null)
            {
                result.Result = GameEnum.WsResult.FinanceLogNotExists;
                return result;
            }
            if (!log.Any())
            {
                result.Result = GameEnum.WsResult.FinanceLogNotExists;
                return result;
            }

            foreach (var l in log)
            {
                result.FinanceLog.Add(new LoadFinanceLogInfo()
                {
                    Count = l.Count,
                    EventName = l.EventName,
                    MoneyType = l.MoneyType,
                    Time = l.Time.ToString(),
                    Type = (int)l.Type
                });
            }

            return result;
        }


        public async Task UpdateFinanceLog(Guid roleId, FinanceLogData log, ITransaction tx)
        {
            var loglist = await FinanceLogListDataHelper.Instance.Get(roleId, tx);

            if (loglist == null)
            {
                loglist = new List<FinanceLogData>();
            }
            loglist.Add(log);
            await FinanceLogListDataHelper.Instance.Update(roleId, loglist, tx);
        }
        public async Task UpdateFinanceLog(Guid roleId, FinanceLogData log)
        {
            var loglist = await FinanceLogListDataHelper.Instance.Get(roleId);

            if (loglist == null)
            {
                loglist = new List<FinanceLogData>();
            }
            loglist.Add(log);
            await FinanceLogListDataHelper.Instance.Update(roleId, loglist);
        }
    }
}
