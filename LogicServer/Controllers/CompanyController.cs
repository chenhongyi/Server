using LogicServer.Data;
using Microsoft.ServiceFabric.Data;
using Model.Data.Npc;
using Model.ResponseData;
using PublicGate.Interface;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Data.Business;
using AutoData;
using GameEnum;

namespace LogicServer.Controllers
{
    public class CompanyController : BaseInstance<CompanyController>
    {

        private async Task<bool> CheckCompanyName(IReliableStateManager sm, string name)
        {
            return await DataHelper.CheckCompanyName(sm, name);
        }
        private async Task<int> CompanyLevelUp(IReliableStateManager sm, Guid roleId, Company cmp)
        {
            return await DataHelper.CompanyLevelUp(sm, roleId, cmp);
        }

        private async Task<bool> CheckCompanyExists(IReliableStateManager sm, Guid roleId)
        {
            return await DataHelper.CheckCompanyExists(sm, roleId);
        }




        public async Task<DepartmentUpdateResult> DepartmentLvUp(IReliableStateManager sm, UserRole role, DepartmentUpdateResult result, DepartMentType type)
        {
            var depart = await DataHelper.GetDepartmentInfoByRoleId(sm, role.Id);
            var bg = await DataHelper.GetRoleBagByRoleIdAsync(sm, role.Id);
            if (bg == null) { result.Result = WsResult.RoleBagErr; return result; }
            if (depart == null) { result.Result = WsResult.DepartmentInvalid; return result; }
            if (!CheckDepartmentLvUp(sm, type, ref depart, role.Id)) { result.Result = WsResult.DepartmentLvUpFailed; return result; }
            int departId = 100000;
            switch (type)
            {
                case DepartMentType.Finance:
                    departId += depart.Finance.Level + 1 + 30000;
                    break;
                case DepartMentType.Personnel:
                    departId += depart.Personnel.Level + 1;
                    break;
                case DepartMentType.Market:
                    departId += depart.Market.Level + 1 + 20000;
                    break;
                case DepartMentType.Investment:
                    departId += depart.Investment.Level + 1 + 10000;
                    break;
            }
            var level = await DepartmentLvUp(sm, role.Id, departId, type);
            if (level == null)
            {
                result.Result = WsResult.DepartmentLvUpFailed;
                return result;
            }
            return result;
        }


        private async Task<DepartmentUpdateResult> DepartmentLvUp(IReliableStateManager sm, Guid id, int departId, DepartMentType type)
        {
            DepartmentUpdateResult result = new DepartmentUpdateResult();
            var config = DepartmentInfo.GetForId(departId);
            var depart = await DataHelper.GetDepartmentInfoByRoleId(sm, id);
            var level = 0;
            if (config == null)
            {
                result.Result = WsResult.ConfigErr;
                return result;
            }
            if (depart == null)
            {
                result.Result = WsResult.DepartmentInvalid;
                return result;
            }
            switch (type)
            {
                case DepartMentType.Finance:
                    level = depart.Finance.Level = depart.Finance.Level + 1;
                    break;
                case DepartMentType.Personnel:
                    level = depart.Personnel.Level = depart.Personnel.Level + 1;
                    break;
                case DepartMentType.Market:
                    level = depart.Market.Level = depart.Market.Level + 1;
                    break;
                case DepartMentType.Investment:
                    level = depart.Investment.Level = depart.Investment.Level + 1;
                    break;
            }
            await DataHelper.DepartmentLvUp(sm, id, depart, departId);
            result.Level = level;
            result.Type = (int)type;
            return result;
        }

        private bool CheckDepartmentLvUp(IReliableStateManager sm, DepartMentType type, ref DepartmentGroup depart, Guid id)
        {
            var company = DataHelper.GetCompanyInfoByRoleId(sm, id).Result; if (company == null) { return false; }
            var bg = DataHelper.GetRoleBagByRoleIdAsync(sm, id).Result; if (bg == null) { return false; }
            GetDepartLvUpData(depart, type, out int level, out int departId, out int SupervisorCount);
            var config = DepartmentInfo.GetForId(departId + 1);
            if (company.Level >= config.level && bg.Items.First(p => p.Id == (int)GameEnum.Currency.Gold).CurCount >= config.CostGold && SupervisorCount >= config.DirectorCounts)
            {
                return true;
            }
            return false;
        }

        private static void GetDepartLvUpData(DepartmentGroup depart, DepartMentType type, out int level, out int departId, out int SupervisorCount)
        {
            level = 0;
            departId = 100000;
            SupervisorCount = 0;
            switch (type)
            {
                case DepartMentType.Finance:
                    level = depart.Finance.Level;
                    departId = 100000 + 30000 + level;
                    SupervisorCount = depart.Finance.CurDirectorCounts;
                    break;
                case DepartMentType.Personnel:
                    level = depart.Personnel.Level;
                    departId = 100000 + level;
                    SupervisorCount = depart.Finance.CurDirectorCounts;
                    break;
                case DepartMentType.Market:
                    level = depart.Market.Level;
                    departId = 100000 + 20000 + level;
                    SupervisorCount = depart.Finance.CurDirectorCounts;
                    break;
                case DepartMentType.Investment:
                    level = depart.Investment.Level;
                    departId = 100000 + 10000 + level;
                    SupervisorCount = depart.Finance.CurDirectorCounts;
                    break;
            }
        }

        public async Task<CreateCompanyResult> CreateCompany(IReliableStateManager sm, UserRole role, string name, CreateCompanyResult result)
        {
            if (string.IsNullOrEmpty(name))
            {
                result.Result = Model.WsResult.ParamsError;
                return result;
            }
            if (await CheckCompanyName(sm, name))
            {
                result.Result = Model.WsResult.DuplicationName;
                return result;
            }
            if (await CheckCompanyExists(sm, role.Id))
            {
                result.Result = Model.WsResult.MoreCompany;
                return result;
            }
            result = await DataHelper.CreateCompany(sm, role, name, result);
            return result;
        }
        internal async Task<LoadDepartMentInfo> GetDepartmentInfoByRoleId(IReliableStateManager sm, Guid id)
        {
            var depart = await DataHelper.GetDepartmentInfoByRoleId(sm, id);
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
        public async Task<LoadCompanyInfo> GetCompanyInfoByRoleId(IReliableStateManager sm, Guid roleId)
        {
            var r = await DataHelper.GetCompanyInfoByRoleId(sm, roleId);
            if (r == null)
            {
                return null;
            }
            LoadCompanyInfo info = new LoadCompanyInfo()
            {
                CreateTime = r.CreateTime.ToString(),
                CurExp = r.CurExp,
                Id = r.Id.ToString(),
                Level = r.Level,
                Name = r.Name,
                //CurFinanceLv = r.CurFinanceLv,
                //CurInvestmentLv = r.CurInvestmentLv,
                //CurMarketLv = r.CurMarketLv,
                //CurPersonnelLv = r.CurPersonnelLv,
            };
            return info;
        }

        public async Task<CompanyLvUpResult> CompanyLvUp(IReliableStateManager sm, UserRole role, CompanyLvUpResult result)
        {
            //公司信息
            var company = await DataHelper.GetCompanyInfoByRoleId(sm, role.Id);
            var bg = await DataHelper.GetRoleBagByRoleIdAsync(sm, role.Id);
            var cmpConfig = CompanyInfo.GetForId(company.Level);
            var department = await DataHelper.GetDepartmentInfoByRoleId(sm, role.Id);
            if (company == null || department == null)
            {
                result.Result = WsResult.NotFoundCompany;
                return result;
            }
            if (CheckCompanyLvUp(company, bg, department))
            {
                await DataHelper.DecGold(sm, role.Id, cmpConfig.CostGold, Currency.Gold); //减钱
                result.Level = await CompanyLevelUp(sm, role.Id, company);
                return result;
            }
            else
            {
                result.Result = WsResult.CompanyLvUpFailed;
                return result;
            }
        }


        private bool CheckCompanyLvUp(Company r, Bag bg, DepartmentGroup department)
        {
            var cmp = CompanyInfo.GetForId(r.Level);
            var count = bg.Items.First(p => p.Id == (int)Currency.Gold).CurCount;
            if (department.Finance.Level >= cmp.FinanceLv && department.Investment.Level >= cmp.InvestmentLv && department.Market.Level >= cmp.MarketLv
                && department.Personnel.Level >= cmp.PersonnelLv && count >= cmp.CostGold && r.CurExp >= cmp.TotalRevenue)
            {
                return true;
            }
            return false;
        }
    }
}
