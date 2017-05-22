using Microsoft.ServiceFabric.Data;
using Model.Data.Business;
using Model.ResponseData;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data
{
    public class FinanceLogController : BaseInstance<FinanceLogController>
    {


        public async Task<List<LoadFinanceLogInfo>> GetFinanceLog(IReliableStateManager sm, Guid roleId)
        {
            List<LoadFinanceLogInfo> result = new List<LoadFinanceLogInfo>();

            var ret = await LoadFinanceLog(sm, roleId);
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
        private async Task<LoadFinanceLogResult> LoadFinanceLog(IReliableStateManager sm, Guid roleId)
        {
            LoadFinanceLogResult result = new LoadFinanceLogResult();
            var log = await DataHelper.GetFinanceLogByRoleIdAsync(sm, roleId);
            if (log == null)
            {
                result.Result = Model.WsResult.FinanceLogNotExists;
                return result;
            }
            if (!log.Any())
            {
                result.Result = Model.WsResult.FinanceLogNotExists;
                return result;
            }

#if DEBUG
            #region Debug
            result.FinanceLog.Add(new LoadFinanceLogInfo()
            {
                Type = 1,
                Count = 1000,
                EventName = "",
                MoneyType = 2,
                Time = DateTime.Now.ToString()
            });
            result.FinanceLog.Add(new LoadFinanceLogInfo()
            {
                Type = 2,
                Count = 1000,
                EventName = "",
                MoneyType = 2,
                Time = DateTime.Now.ToString()
            });
            result.FinanceLog.Add(new LoadFinanceLogInfo()
            {
                Type = 3,
                Count = -1000,
                EventName = "时装名称",
                MoneyType = 2,
                Time = DateTime.Now.ToString()
            });
            result.FinanceLog.Add(new LoadFinanceLogInfo()
            {
                Type = 4,
                Count = 1000,
                EventName = "道具名称",
                MoneyType = 2,
                Time = DateTime.Now.ToString()
            });
            result.FinanceLog.Add(new LoadFinanceLogInfo()
            {
                Type = 5,
                Count = -1000,
                EventName = "道具名称",
                MoneyType = 2,
                Time = DateTime.Now.ToString()
            });
            #endregion
#else


            foreach (var l in log)
            {
                result.FinanceLog.Add(new FinanceLog()
                {
                    Count = l.Count,
                    EventName = l.EventName,
                    MoneyType = l.MoneyType,
                    Time = l.Time.ToString(),
                    Type = l.Type
                });
            }
#endif
            return result;
        }

        public async Task UpdateFinanceLog(IReliableStateManager sm, Guid roleId, FinanceLogData log)
        {
            if (log == null) throw new ArgumentNullException();
            await DataHelper.UpdateFinanceLogByRoleIdAsync(sm, roleId, log);
            await MsgSender.Instance.FinanceLogUpdate(sm, roleId, log);
        }

        public async Task UpdateFinanceLog(IReliableStateManager sm, Guid roleId, List<FinanceLogData> log)
        {
            if (log == null) throw new ArgumentNullException();
            await DataHelper.UpdateFinanceLogByRoleIdAsync(sm, roleId, log);
            foreach (var item in log)
            {
                await MsgSender.Instance.FinanceLogUpdate(sm, roleId, item);
            }
        }
    }
}
