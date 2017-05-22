using GameEnum;
using Microsoft.ServiceFabric.Data;
using Model;
using Model.MsgQueue;
using Model.ResponseData;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Data.Business;

namespace LogicServer.Data
{
    public class MsgSender : BaseInstance<MsgSender>
    {

        public async Task ItemUpdate(IReliableStateManager sm, Guid roleId, int itemId, long count)
        {

        }

        public async Task GoldUpdate(IReliableStateManager sm, Guid roleId, long count, Currency type)
        {
            GoldChangedResult result = new GoldChangedResult()
            {
                Count = count,
                GoldType = (int)type
            };
            var data = await InitHelpers.GetPse().SerializeAsync(result);
            MsgQueueList msg = new MsgQueueList();
            await MsgMaker.SendMessage(WSResponseMsgID.GoldChangedResult, 1, roleId, sm, data);

        }

        internal async Task FinanceLogUpdate(IReliableStateManager sm, Guid roleId, FinanceLogData log)
        {
            TCFinanceLogChangedResult result = new TCFinanceLogChangedResult()
            {
                FinanceLogInfo = new LoadFinanceLogInfo()
                {
                    Count = log.Count,
                    EventName = log.EventName,
                    MoneyType = log.MoneyType,
                    Time = log.Time.ToString(),
                    Type = (int)log.Type
                }
            };
            var data = await InitHelpers.GetPse().SerializeAsync(result);
            MsgQueueList msg = new MsgQueueList();
            await MsgMaker.SendMessage(WSResponseMsgID.TCFinanceLogChangedResult, 1, roleId, sm, data);
        }

    }
}
