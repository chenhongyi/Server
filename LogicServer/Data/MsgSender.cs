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
using LogicServer.Data.Helper;

namespace LogicServer.Data
{
    public class MsgSender : BaseInstance<MsgSender>
    {

        public async Task ItemUpdate(int itemId, long count)
        {

        }



        /// <summary>
        /// 金钱变动消息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="count"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task GoldUpdate(int type)
        {

            GoldChangedResult result = new GoldChangedResult()
            {
                GoldType = type
            };
            var bag = LogicServer.User.bag;
            if (bag.Items.TryGetValue(type, out Model.Data.General.Item money))
            {
                result.Count = money.CurCount;
            }
            var data = await InitHelpers.GetPse().SerializeAsync(result);
            MsgQueueList msg = new MsgQueueList();
            await MsgMaker.SendMessage(WSResponseMsgID.GoldChangedResult, 1, data);
        }




        /// <summary>
        /// 更新身价
        /// </summary>
        /// <param name="income"></param>
        /// <returns></returns>
        public async Task UpdateIncome()
        {
            UpdateShenjiaResult result = new UpdateShenjiaResult();
            result.SocialStatus = LogicServer.User.role.SocialStatus;
            var data = await InitHelpers.GetPse().SerializeAsync(result);
            MsgQueueList msg = new MsgQueueList();
            await MsgMaker.SendMessage(WSResponseMsgID.UpdateShenjiaResult, 1, data);
        }



        internal async Task FinanceLogUpdate(FinanceLogData log)
        {
            TCFinanceLogChangedResult result = new TCFinanceLogChangedResult()
            {
                FinanceLogInfo = new LoadFinanceLogInfo()
                {
                    Count = log.Count,
                    EventName = log.EventName,
                    MoneyType = log.MoneyType,
                    Time = log.Time.ToString(),
                    Type = (int)log.Type,
                    AorD = log.AorD
                }
            };
            var data = await InitHelpers.GetPse().SerializeAsync(result);
            MsgQueueList msg = new MsgQueueList();
            await MsgMaker.SendMessage(WSResponseMsgID.TCFinanceLogChangedResult, 1, data);
        }


    }
}
