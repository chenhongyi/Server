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
using LogicServer.Data.Helper;

namespace LogicServer.Controllers
{
    public class CompanyController : BaseInstance<CompanyController>
    {
        public async Task<CreateCompanyResult> CreateCompany(Guid roleId, string name, CreateCompanyResult result)
        {

            Company company = new Company(name);
            await CompanyDataHelper.Instance.SetCompanyByRoleIdAsync(roleId, company);
            result.DepartmentInfo = await DepartmentController.Instance.CreateDepartment();    //创建部门并初始化

            result.CompanyInfo.Id = company.Id.ToString();
            result.CompanyInfo.Level = company.Level;
            result.CompanyInfo.Name = company.Name;
            result.CompanyInfo.CurExp = company.CurExp;

            result.CompanyInfo.CreateTime = company.CreateTime.Date.ToString();
            return result;

        }

        private async Task<bool> CheckCompanyName(string name)
        {
            return await CompanyNameDataHelper.Instance.CheckCompanyIdByName(name);
        }

        private async Task<int> CompanyLevelUp(Guid roleId, Company cmp, int goldCount, int goldType)
        {
            cmp.Level++;
            var cmpConfig = CompanyInfo.GetForId(cmp.Level);
            var role = LogicServer.User.role;
            role.SocialStatus += cmpConfig.Income;
            FinanceLogData log = new FinanceLogData()
            {
                Count = -goldCount,
                MoneyType = goldType,
                Type = (int)GameEnum.FinanceLog.LvUpCompany,
                AorD = false
            };
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await BagDataHelper.Instance.DecGold(goldCount, goldType, tx);  //减钱
                await CompanyDataHelper.Instance.UpdateCompanyByRoleIdAsync(roleId, cmp, tx);   //升级
                await RoleDataHelper.Instance.UpdateRoleByRoleIdAsync(roleId, role, tx);            //更新身价
                await FinanceLogController.Instance.UpdateFinanceLog(roleId, log, tx);
                await tx.CommitAsync();
            }
            await MsgSender.Instance.UpdateIncome();    //发送身价变动消息
            await MsgSender.Instance.GoldUpdate(goldType);                                                            //发送金钱变动消息
            await MsgSender.Instance.FinanceLogUpdate(log);


            return cmp.Level;
        }



        private async Task<bool> CheckCompanyExists(Guid roleId)
        {
            return await CompanyDataHelper.Instance.CheckCompanyByRoleId(roleId);
        }





        public async Task<CreateCompanyResult> CreateCompany(IReliableStateManager sm, UserRole role, string name, CreateCompanyResult result)
        {
            if (string.IsNullOrEmpty(name))
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            if (await CheckCompanyName(name))
            {
                result.Result = GameEnum.WsResult.DuplicationName;
                return result;
            }
            if (await CheckCompanyExists(role.Id))
            {
                result.Result = GameEnum.WsResult.MoreCompany;
                return result;
            }
            result = await CreateCompany(role.Id, name, result);
            return result;
        }

        public async Task<LoadCompanyInfo> GetCompanyInfoByRoleId(Guid roleId)
        {
            var r = await CompanyDataHelper.Instance.GetCompanyByRoleIdAsync(roleId);
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
            };
            return info;
        }

        public async Task<CompanyLvUpResult> CompanyLvUp(CompanyLvUpResult result)
        {
            var role = LogicServer.User.role;
            var bg = LogicServer.User.bag;


            var company = await CompanyDataHelper.Instance.GetCompanyByRoleIdAsync(role.Id);
            var department = await DepartmentGroupDataHelper.Instance.GetDepartMentGroupByRoleId(role.Id);
            var cmpConfig = CompanyInfo.GetForId(company.Level + 1);

            if (company == null || department == null)
            {
                result.Result = GameEnum.WsResult.NotFoundCompany;
                return result;
            }
            if (CheckCompanyLvUp(company, department))
            {

                result.Level = await CompanyLevelUp(role.Id, company, cmpConfig.CostGold, (int)Currency.Gold);
                return result;
            }
            else
            {
                result.Result = GameEnum.WsResult.CompanyLvUpFailed;
                return result;
            }
        }


        private bool CheckCompanyLvUp(Company r, DepartmentGroup department)
        {
            var bg = LogicServer.User.bag;
            var cmp = CompanyInfo.GetForId(r.Level);
            bg.Items.TryGetValue((int)Currency.Gold, out Model.Data.General.Item count);
            if (department.Finance.Level >= cmp.FinanceLv && department.Investment.Level >= cmp.InvestmentLv && department.Market.Level >= cmp.MarketLv
                && department.Personnel.Level >= cmp.PersonnelLv && count.CurCount >= cmp.CostGold && r.CurExp >= cmp.TotalRevenue)
            {
                return true;
            }
            return false;
        }
    }
}
