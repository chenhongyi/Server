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
using Model.Protocol;
using Model.Data.Npc;

namespace LogicServer.Data
{
    public  class MsgSender : BaseInstance<MsgSender>
    {

        public  async Task ItemUpdate(int itemId, long count)
        {
            var result = new UpdateItemResult() { Items = new List<ItemInfoCls>() { new ItemInfoCls() { ItemId = itemId, Count = count } } };
            await MsgMaker.SendMessage(WSResponseMsgID.UpdateItemResult, result);
        }


        public  async Task UseItem(int itemId, long count)
        {
            var result = await Controllers.BagController.Instance.UseItemsAsync(null, Guid.Empty, itemId, count);
            await MsgMaker.SendMessage(WSResponseMsgID.AddItemResult, result);
        }


        /// <summary>
        /// 金钱变动消息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="count"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public  async Task UpdateGold(int type)
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
            await MsgMaker.SendMessage(WSResponseMsgID.GoldChangedResult, 1, data);
        }




        /// <summary>
        /// 更新身价
        /// </summary>
        /// <param name="income"></param>
        /// <returns></returns>
        public  async Task UpdateIncome()
        {
            UpdateShenjiaResult result = new UpdateShenjiaResult();
            result.SocialStatus = LogicServer.User.role.SocialStatus;
            var data = await InitHelpers.GetPse().SerializeAsync(result);
            await MsgMaker.SendMessage(WSResponseMsgID.UpdateShenjiaResult, 1, data);
        }


        public  async Task BuildExtendFailed(BuildExtendFailedResult result)
        {
            var data = await InitHelpers.GetPse().SerializeAsync(result);
            await MsgMaker.SendMessage(WSResponseMsgID.BuildExtendFailedResult, 1, data);
        }


        public  async Task FinanceLogUpdate(FinanceLogData log)
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
            await MsgMaker.SendMessage(WSResponseMsgID.TCFinanceLogChangedResult, 1, data);
        }


        /// <summary>
        /// 更新部门之投资部
        /// </summary>
        /// <param name="department"></param>
        /// <returns></returns>

        public  async Task UpdateDepartmentInvestment(Department department)
        {
            TCDepartmentInvestmentResult result = new TCDepartmentInvestmentResult()
            {
                InvestmentInfo = new InvestmentInfo()
                {
                    CurDirectorCounts = department.CurDirectorCounts,
                    CurExtension = department.CurExtension,
                    CurRealestate = department.CurRealestate,
                    CurStaff = department.CurStaff,
                    CurStore = department.CurStore,
                    Level = department.Level
                }
            };
            var data = await InitHelpers.GetPse().SerializeAsync(result);
            await MsgMaker.SendMessage(WSResponseMsgID.TCDepartmentInvestmentResult, 1, data);
        }

        public  async Task BuildLvUpFailed(BuildLvUpFailedResult failed)
        {
            var data = await InitHelpers.GetPse().SerializeAsync(failed);
            await MsgMaker.SendMessage(WSResponseMsgID.BuildLvUpFailedResult, 1, data);
        }

        public  async Task UpdateBuildInfo(BuildData buildData)
        {
            if (buildData != null)
            {
                TCBuildUpdateResult result = new TCBuildUpdateResult()
                {
                    LandBuildInfo = new LoadBuildInfo()
                    {
                        BuildId = buildData.Id,
                        BuildType = buildData.BuildType,
                        CurExtendLv = buildData.CurExtendLv,
                        CustomerAddtion = buildData.CustomerAddtion,
                        Employee = buildData.Employee,
                        GetMoney = buildData.GetMoney,
                        Level = buildData.Level,
                        Name = buildData.Name,
                        Popularity = buildData.Popularity,
                        Pos = buildData.Pos,
                        RoleId = buildData.RoleId,
                        Star = buildData.Star,
                        TodayCanAdvartise = buildData.TodayCanAdvartise,
                        CostGold = buildData.CostGold,
                        Income = buildData.Income

                    }
                };
                var data = await InitHelpers.GetPse().SerializeAsync(result);
                await MsgMaker.SendMessage(WSResponseMsgID.TCBuildUpdateResult, 1, data);
            }
        }
    }
}
