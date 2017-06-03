using AutoData;
using LogicServer.Data;
using LogicServer.Data.Helper;
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
    public class DepartmentController : BaseInstance<DepartmentController>
    {
        public async Task DepartmentLvUp(DepartmentGroup depart, int departid)
        {
            var role = LogicServer.User.role;
            var config = DepartmentInfo.GetForId(departid);
            var oldconfig = DepartmentInfo.GetForId(departid - 1);
            long updateIncome = config.Income - oldconfig.Income;
            FinanceLogData log = new FinanceLogData()
            {
                Count = -oldconfig.CostGold.Count,
                MoneyType = (int)GameEnum.Currency.Gold,
                Type = (int)GameEnum.FinanceLog.LvUpDepartment,
                AorD = false
            };
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await DepartmentGroupDataHelper.Instance.UpdateDepartMentGroupByRoleId(role.Id, depart, tx);
                await BagDataHelper.Instance.DecGold(oldconfig.CostGold.Count, oldconfig.CostGold.Type, tx); //减钱
                await RoleController.Instance.AddIncome(role.Id, updateIncome,tx);
                await FinanceLogController.Instance.UpdateFinanceLog(role.Id, log, tx);
                await tx.CommitAsync();
            }
            await MsgSender.Instance.UpdateGold(config.CostGold.Type);
            await MsgSender.Instance.UpdateIncome();  //更新身价
            await MsgSender.Instance.FinanceLogUpdate(log);
        }


        public async Task<LoadDepartMentInfo> CreateDepartment()
        {
            DepartmentGroup departments = new DepartmentGroup();
            var role = LogicServer.User.role;
            await DepartmentGroupDataHelper.Instance.SetDepartMentGroupByRoleId(role.Id, departments);
            var p = DepartmentInfo.GetForId(100001);
            var f = DepartmentInfo.GetForId(110001);
            var i = DepartmentInfo.GetForId(120001);
            var m = DepartmentInfo.GetForId(130001);

            var shenjia = p.Income + f.Income + i.Income + m.Income;

            await MsgSender.Instance.UpdateIncome(); //更新身价

            LoadDepartMentInfo result = new LoadDepartMentInfo()
            {
                FinanceInfo = new FinanceInfo()
                {
                    CurDirectorCounts = departments.Finance.CurDirectorCounts,
                    CurStaff = departments.Finance.CurStaff,
                    Level = departments.Finance.Level,
                    MakerCoinCounts = departments.Finance.MakerCoinCounts
                },

                InvestmentInfo = new InvestmentInfo()
                {
                    CurStaff = departments.Investment.CurStaff,
                    Level = departments.Investment.Level,
                    CurDirectorCounts = departments.Investment.CurDirectorCounts,
                    CurExtension = departments.Investment.CurExtension,
                    CurRealestate = departments.Investment.CurRealestate,
                    CurStore = departments.Investment.CurStore
                },
                MarketInfo = new MarketInfo()
                {
                    CurDirectorCounts = departments.Market.CurDirectorCounts,
                    Level = departments.Market.Level,
                    CurStaff = departments.Market.CurStaff,
                    CurPropagandaCounts = departments.Market.CurPropagandaCounts,
                    CurPurchaseCounts = departments.Market.CurPurchaseCounts,
                    CurStoreAddtion = departments.Market.CurStoreAddtion,
                    CurStrategicCounts = departments.Market.CurStrategicCounts
                },
                PersonnelInfo = new PersonnelInfo()
                {
                    CurStaff = departments.Personnel.CurStaff,
                    Level = departments.Personnel.Level,
                    CurDirectorCounts = departments.Personnel.CurDirectorCounts,
                    CurTalentLv = departments.Personnel.CurTalentLv
                }
            };

            return result;
        }


        public async Task<LoadDepartMentInfo> GetDepartmentInfoByRoleId(Guid roleId)
        {
            var depart = await DepartmentGroupDataHelper.Instance.GetDepartMentGroupByRoleId(roleId);
            if (depart == null)
            {
                return null;
            }
            LoadDepartMentInfo info = new LoadDepartMentInfo()
            {
                FinanceInfo = new FinanceInfo()
                {
                    Level = depart.Finance.Level,
                    CurStaff = depart.Finance.CurStaff,
                    MakerCoinCounts = depart.Finance.MakerCoinCounts,
                    CurDirectorCounts = depart.Finance.CurDirectorCounts
                },
                InvestmentInfo = new InvestmentInfo()
                {
                    Level = depart.Investment.Level,
                    CurDirectorCounts = depart.Investment.CurDirectorCounts,
                    CurExtension = depart.Investment.CurExtension,
                    CurRealestate = depart.Investment.CurRealestate,
                    CurStaff = depart.Investment.CurStaff,
                    CurStore = depart.Investment.CurStore
                },
                MarketInfo = new MarketInfo()
                {
                    CurStaff = depart.Market.CurStaff,
                    CurDirectorCounts = depart.Market.CurDirectorCounts,
                    Level = depart.Market.Level,
                    CurPropagandaCounts = depart.Market.CurPropagandaCounts,
                    CurPurchaseCounts = depart.Market.CurPurchaseCounts,
                    CurStoreAddtion = depart.Market.CurStoreAddtion,
                    CurStrategicCounts = depart.Market.CurStrategicCounts
                },
                PersonnelInfo = new PersonnelInfo()
                {
                    Level = depart.Personnel.Level,
                    CurDirectorCounts = depart.Personnel.CurDirectorCounts,
                    CurStaff = depart.Personnel.CurStaff,
                    CurTalentLv = depart.Personnel.CurTalentLv
                }
            };
            return info;
        }


        public async Task<DepartmentUpdateResult> DepartmentLvUp(DepartmentUpdateResult result, GameEnum.DepartMentType type)
        {
            var role = LogicServer.User.role;
            var depart = await DepartmentGroupDataHelper.Instance.GetDepartMentGroupByRoleId(role.Id);
            var bg = LogicServer.User.bag;
            if (depart == null) { result.Result = GameEnum.WsResult.DepartmentInvalid; return result; }
            if (!CheckDepartmentLvUp(role.Id, type, ref depart)) { result.Result = GameEnum.WsResult.DepartmentLvUpFailed; return result; }
            int departId = 100000;
            switch (type)
            {
                case GameEnum.DepartMentType.Finance:
                    departId += depart.Finance.Level + 1 + 30000;
                    break;
                case GameEnum.DepartMentType.Personnel:
                    departId += depart.Personnel.Level + 1;
                    break;
                case GameEnum.DepartMentType.Market:
                    departId += depart.Market.Level + 1 + 20000;
                    break;
                case GameEnum.DepartMentType.Investment:
                    departId += depart.Investment.Level + 1 + 10000;
                    break;
            }
            result = await DepartmentLvUp(role.Id, departId, type);
            if (result == null)
            {
                result.Result = GameEnum.WsResult.DepartmentLvUpFailed;
                return result;
            }
            return result;
        }


        private async Task<DepartmentUpdateResult> DepartmentLvUp(Guid roleId, int departId, GameEnum.DepartMentType type)
        {
            DepartmentUpdateResult result = new DepartmentUpdateResult();
            var config = DepartmentInfo.GetForId(departId);
            var depart = await DepartmentGroupDataHelper.Instance.GetDepartMentGroupByRoleId(roleId);
            var level = 0;
            if (config == null)
            {
                result.Result = GameEnum.WsResult.ConfigErr;
                return result;
            }
            if (depart == null)
            {
                result.Result = GameEnum.WsResult.DepartmentInvalid;
                return result;
            }
            switch (type)
            {
                case GameEnum.DepartMentType.Finance:
                    level = depart.Finance.Level = depart.Finance.Level + 1;
                    break;
                case GameEnum.DepartMentType.Personnel:
                    level = depart.Personnel.Level = depart.Personnel.Level + 1;
                    break;
                case GameEnum.DepartMentType.Market:
                    level = depart.Market.Level = depart.Market.Level + 1;
                    break;
                case GameEnum.DepartMentType.Investment:
                    level = depart.Investment.Level = depart.Investment.Level + 1;
                    break;
            }
            await DepartmentController.Instance.DepartmentLvUp(depart, departId);
            result.Level = level;
            result.Type = (int)type;
            return result;
        }

        private bool CheckDepartmentLvUp(Guid roleId, GameEnum.DepartMentType type, ref DepartmentGroup depart)
        {
            var company = CompanyDataHelper.Instance.GetCompanyByRoleIdAsync(roleId).Result; if (company == null) { return false; }
            var bg = LogicServer.User.bag;
            GetDepartLvUpData(depart, type, out int level, out int departId, out int SupervisorCount);
            var config = DepartmentInfo.GetForId(departId + 1);
            bg.Items.TryGetValue(config.CostGold.Type, out Model.Data.General.Item money);
            if (company.Level >= config.level && money.CurCount >= config.CostGold.Count && SupervisorCount >= config.DirectorCounts)
            {
                return true;
            }
            return false;
        }

        private static void GetDepartLvUpData(DepartmentGroup depart, GameEnum.DepartMentType type, out int level, out int departId, out int SupervisorCount)
        {
            level = 0;
            departId = 100000;
            SupervisorCount = 0;
            switch (type)
            {
                case GameEnum.DepartMentType.Finance:
                    level = depart.Finance.Level;
                    departId = 100000 + 30000 + level;
                    SupervisorCount = depart.Finance.CurDirectorCounts;
                    break;
                case GameEnum.DepartMentType.Personnel:
                    level = depart.Personnel.Level;
                    departId = 100000 + level;
                    SupervisorCount = depart.Finance.CurDirectorCounts;
                    break;
                case GameEnum.DepartMentType.Market:
                    level = depart.Market.Level;
                    departId = 100000 + 20000 + level;
                    SupervisorCount = depart.Finance.CurDirectorCounts;
                    break;
                case GameEnum.DepartMentType.Investment:
                    level = depart.Investment.Level;
                    departId = 100000 + 10000 + level;
                    SupervisorCount = depart.Finance.CurDirectorCounts;
                    break;
            }
        }

    }
}
