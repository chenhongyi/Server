using AutoData;
using Microsoft.ServiceFabric.Data.Collections;
using Model.Data.Account;
using Model.Data.Npc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Model.Data.General;
using Model;
using Model.MsgQueue;
using Shared.Serializers;
using Shared;
using Model.Data.Business;
using GameEnum;
using Model.ResponseData;

namespace LogicServer.Data
{

    public class DataHelper
    {

        #region 数据表名定义
        private const string AccountsCollectionName = "accounts";



        private const string TokenCollectionName = "tokens";
        private const string PassportCollectionName = "passports";
        private const string LoginCollectionName = "logins";



        private const string OnlinePlayerName = "onlinePlayers";
        private const string SessionCollectionName = "sessionUserNames";
        private const string UserNameSessionName = "userNameSessions";



        private const string RoleName = "ckRoles";
        private const string AccountRoleName = "accountRoles";                      //账号和用户名绑定
        private const string IdRoleName = "idRoles";                                //账号id和用户名绑定
        private const string AIdBindAccountName = "aidBindAccount";                 //账号id绑定账号名
        private const string UserNameBindAccountIdName = "userNameBindAccountId";   //用户名绑定账号
        private const string AccoutIdBindUserNameName = "accoutIdBindUserName";     //账号绑定用户名
        private const string RoleIdBindUserAttrsName = "roleIdBindUserAttrs";       //角色名绑定属性


        private const string RoleIdBindBagName = "roleIdBindStoreageBox";    //角色名绑定背包
        private const string SessionBindRoleIdName = "sessionBindRole";               //通过session绑定角色名


        private const string RoleIdBindSessionName = "roleIdBindSession";
        //private const string SessionBindTokenName = "sessionBindToken";             //通过session绑定token
        private const string MsgQueueListName = "msgQueneListName";                 //等待发送消息队列
        private const string RoleIdBindCompanyName = "roleIdBindCompany";
        private const string CompanyNameBindCompanyIdName = "companyNameBindCompanyId"; //公司名字绑定公司id    用于检查重名
        private const string RoleIdBindDepartmentName = "roleIdBindDepartment";
        private const string RoleIdBindFinanceLogName = "roleIdBindFinanceLog";

        /// <summary>
        /// key = roleid, value = List<FinanceLogData>
        /// </summary>
        private static IReliableDictionary<Guid, List<FinanceLogData>> roleIdBindFinanceLogList = null;

        /// <summary>
        /// 角色名绑定公司各部门  key = roleid, value = departmentgroup
        /// </summary>
        private static IReliableDictionary<Guid, DepartmentGroup> roleIdBindDepartment = null;

        /// <summary>
        ///  key = 公司名称 string  value = 公司id Guid
        /// </summary>
        private static IReliableDictionary<string, Guid> companyNameBindCompanyID = null;

        /// <summary>
        /// key = roleid  value = Company
        /// </summary>
        private static IReliableDictionary<Guid, Company> companyCollection = null;

        /// <summary>
        /// key  =队列id  队列id为2 循环发送   value=消息集合
        /// </summary>
        private static IReliableDictionary<int, List<MsgQueueList>> msgQueueList = null;



        /// <summary>
        /// key = roleID  value = session       //在线角色
        /// </summary>
        private static IReliableDictionary<Guid, string> RoleIdBindSession = null;

        /// <summary>
        /// key = sessionId   value  = UserRole
        /// </summary>
        private static IReliableDictionary<string, UserRole> sessionBindRole = null;

        /// <summary>
        /// key = roleId  value = Bag   角色背包表
        /// </summary>
        private static IReliableDictionary<Guid, Bag> roleIdBindBag = null;
        /// <summary>
        /// key = roleId  value = List<UserAttr>  角色属性表
        /// </summary>
        private static IReliableDictionary<Guid, List<Model.Data.Npc.UserAttr>> roleIdBindUserAttrs = null;



        /// <summary>
        /// key = username  value  = account
        /// </summary>
        private static IReliableDictionary<string, Account> accountsCollection = null;         //账号数据
        private static IReliableDictionary<string, Token> tokenCollection = null;              //token数据  key是 guid.tostring()
        private static IReliableDictionary<string, Passport> passportCollection = null;        //通行证数据
        private static IReliableDictionary<string, Login> loginCollection = null;              //Login数据
                                                                                               //   private static IReliableDictionary<string, Token> sessionBindToken = null;              //token绑定session 登录阶段 Login

        /// <summary>
        /// 账号id绑定用户账号  key = Account.AccountId  value =Account.UserName
        /// </summary>
        private static IReliableDictionary<Guid, string> userNameBindAccountId = null;

        /// <summary>
        /// 用户账号绑定账号id key = Account.UserName value =Account.AccountId 
        /// </summary>
        private static IReliableDictionary<string, Guid> accoutIdBindUserName = null;

        /// <summary>
        /// 账号id  绑定 账号  key = Account.AccountId  value = Account
        /// </summary>
        private static IReliableDictionary<Guid, Account> aIdBindAccount = null;

        /// <summary>
        /// Guid = UserRole.Id string = session  roleid为key  session为value
        /// </summary>
        //private static IReliableDictionary<Guid, string> onlinePlayerCollection = null;        //在线玩家

        /// <summary>
        /// string = UserName string = session  
        /// </summary>
        private static IReliableDictionary<string, string> sessionUserNameCollection = null;             //session和用户名数据绑定   先绑定到账号上 角色登陆后 修改为绑定到角色上  角色下线 再改为账号 断开连接 删除session

        /// <summary>
        /// string =session string = UserName(用户账号 登录账号 User123456)
        /// </summary>
        private static IReliableDictionary<string, string> userNameSessionCollection = null;             //用户名和session绑定
        private static IReliableDictionary<Guid, UserRole> roleCollection = null;        //角色表
        private static IReliableDictionary<Guid, List<UserRole>> accountRoleCollection = null;       //账号和角色绑定表

        /// <summary>
        /// string = 角色名   IdRole = 角色名 +  id
        /// </summary>
        private static IReliableDictionary<string, IdRole> idRoleCollection = null;      //角色id和角色姓名关联表

        #endregion

        internal static async Task<List<FinanceLogData>> GetFinanceLogByRoleIdAsync(IReliableStateManager sm, Guid roleId)
        {
            roleIdBindFinanceLogList = await sm.GetOrAddAsync<IReliableDictionary<Guid, List<FinanceLogData>>>(RoleIdBindFinanceLogName);
            try
            {
                using (var tx = sm.CreateTransaction())
                {
                    var result = await roleIdBindFinanceLogList.TryGetValueAsync(tx, roleId);
                    return result.HasValue ? result.Value : null;
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }


        internal static async Task UpdateFinanceLogByRoleIdAsync(IReliableStateManager sm, Guid roleId, List<FinanceLogData> log)
        {
            roleIdBindFinanceLogList = await sm.GetOrAddAsync<IReliableDictionary<Guid, List<FinanceLogData>>>(RoleIdBindFinanceLogName);
            try
            {
                var oldLog = await GetFinanceLogByRoleIdAsync(sm, roleId);
                if (oldLog == null)
                {
                    List<FinanceLogData> logList = new List<FinanceLogData>();
                    logList.AddRange(log);
                    await BindFinanceLogByRoleIdAsync(sm, roleId, logList);
                }
                else
                {
                    var newLog = oldLog.Where(p => p.Time.Date >= DateTime.Now.AddDays(-7)).ToList();
                    var cmpLog = newLog;
                    newLog.AddRange(log);
                    using (var tx = sm.CreateTransaction())
                    {
                        await roleIdBindFinanceLogList.TryUpdateAsync(tx, roleId, newLog, cmpLog);
                        await tx.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        internal static async Task UpdateFinanceLogByRoleIdAsync(IReliableStateManager sm, Guid roleId, FinanceLogData log)
        {
            roleIdBindFinanceLogList = await sm.GetOrAddAsync<IReliableDictionary<Guid, List<FinanceLogData>>>(RoleIdBindFinanceLogName);
            try
            {
                var oldLog = await GetFinanceLogByRoleIdAsync(sm, roleId);
                if (oldLog == null)
                {
                    List<FinanceLogData> logList = new List<FinanceLogData>();
                    logList.Add(log);
                    await BindFinanceLogByRoleIdAsync(sm, roleId, logList);
                }
                else
                {
                    var newLog = oldLog.Where(p => p.Time.Date >= DateTime.Now.AddDays(-7)).ToList();
                    var cmpLog = newLog;
                    newLog.Add(log);
                    using (var tx = sm.CreateTransaction())
                    {
                        await roleIdBindFinanceLogList.TryUpdateAsync(tx, roleId, newLog, cmpLog);
                        await tx.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }



        internal static async Task BindFinanceLogByRoleIdAsync(IReliableStateManager sm, Guid roleId, List<FinanceLogData> list)
        {
            roleIdBindFinanceLogList = await sm.GetOrAddAsync<IReliableDictionary<Guid, List<FinanceLogData>>>(RoleIdBindFinanceLogName);
            try
            {
                using (var tx = sm.CreateTransaction())
                {
                    await roleIdBindFinanceLogList.AddAsync(tx, roleId, list);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        internal static async Task DepartmentLvUp(IReliableStateManager sm, Guid id, DepartmentGroup depart, int departid)
        {
            var cmp = depart;
            try
            {
                roleIdBindDepartment = await sm.GetOrAddAsync<IReliableDictionary<Guid, DepartmentGroup>>(RoleIdBindDepartmentName);
                using (var tx = sm.CreateTransaction())
                {
                    await roleIdBindDepartment.TryUpdateAsync(tx, id, depart, cmp);
                    await tx.CommitAsync();
                }
                var config = DepartmentInfo.GetForId(departid);
                var oldconfig = DepartmentInfo.GetForId(departid - 1);
                var updateIncome = config.Income - oldconfig.Income;

                await UpdateShenjia(sm, id, updateIncome);  //更新身价
                await DecGold(sm, id, config.CostGold, Currency.Gold); //减钱
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 金钱减少
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="bg">背包</param>
        /// <param name="costGold">减少的数额</param>
        /// <param name="currency">货币类型</param>
        /// <returns></returns>
        internal static async Task DecGold(IReliableStateManager sm, Guid roleId, int costGold, Currency currency)
        {
            try
            {
                var bg = await GetRoleBagByRoleIdAsync(sm, roleId);
                if (bg == null)
                {
                    throw new Exception("该角色背包出错!");
                }
                var cmpbg = bg;
                var money = bg.Items.First(p => p.Id == (int)currency);
                if (money.CurCount >= costGold)
                {
                    money.CurCount -= costGold;
                }
                UpdateRoleBagByRoleIdAsync(sm, roleId, bg, cmpbg).Wait();
                await UpdateGoldMsg(sm, roleId, money.CurCount, currency);

            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        internal static async Task UpdateGoldMsg(IReliableStateManager sm, Guid roleId, long count, Currency type)
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

        /// <summary>
        /// 公司升级
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="cmp"></param>
        /// <returns></returns>
        internal static async Task<int> CompanyLevelUp(IReliableStateManager sm, Guid roleId, Company cmp)
        {
            var old = cmp;
            cmp.Level++;
            var cmpConfig = CompanyInfo.GetForId(cmp.Level);
            try
            {
                companyCollection = await sm.GetOrAddAsync<IReliableDictionary<Guid, Company>>(RoleIdBindCompanyName);
                using (var tx = sm.CreateTransaction())
                {
                    await companyCollection.TryUpdateAsync(tx, roleId, cmp, old);
                    await tx.CommitAsync();
                }
                await UpdateShenjia(sm, roleId, cmpConfig.Income);  //更新身价
                return cmp.Level;
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 更新身价
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="shenjia">增加的身价值</param>
        /// <returns></returns>
        internal static async Task UpdateShenjia(IReliableStateManager sm, Guid roleId, int shenjia)
        {
            var role = await GetRoleInfoByRoleIdAsync(sm, roleId);
            var old = role;
            role.SocialStatus += shenjia;
            await UpdateRoleInfoByRoleIdAsync(sm, roleId, role, old);
            UpdateShenjiaResult result = new UpdateShenjiaResult();
            result.SocialStatus = role.SocialStatus;
            var data = await InitHelpers.GetPse().SerializeAsync(result);
            MsgQueueList msg = new MsgQueueList();
            await MsgMaker.SendMessage(WSResponseMsgID.UpdateShenjiaResult, 1, roleId, sm, data);
        }

        /// <summary>
        /// 获取公司信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        internal static async Task<Company> GetCompanyInfoByRoleId(IReliableStateManager sm, Guid roleId)
        {
            try
            {
                companyCollection = await sm.GetOrAddAsync<IReliableDictionary<Guid, Company>>(RoleIdBindCompanyName);
                using (var tx = sm.CreateTransaction())
                {
                    var result = await companyCollection.TryGetValueAsync(tx, roleId);
                    return result.HasValue ? result.Value : null;
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static async Task<DepartmentGroup> GetDepartmentInfoByRoleId(IReliableStateManager sm, Guid id)
        {
            try
            {
                DepartmentGroup departments = new DepartmentGroup();
                roleIdBindDepartment = await sm.GetOrAddAsync<IReliableDictionary<Guid, DepartmentGroup>>(RoleIdBindDepartmentName);
                using (var tx = sm.CreateTransaction())
                {
                    var info = await roleIdBindDepartment.TryGetValueAsync(tx, id);
                    return info.HasValue ? info.Value : null;
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }



        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private static async Task<LoadDepartMentInfo> CreateDepartment(IReliableStateManager sm, Guid roleId)
        {
            try
            {
                DepartmentGroup departments = new DepartmentGroup();

                roleIdBindDepartment = await sm.GetOrAddAsync<IReliableDictionary<Guid, DepartmentGroup>>(RoleIdBindDepartmentName);
                using (var tx = sm.CreateTransaction())
                {
                    await roleIdBindDepartment.AddAsync(tx, roleId, departments);
                    await tx.CommitAsync();
                }
                var p = DepartmentInfo.GetForId(100001);
                var f = DepartmentInfo.GetForId(110001);
                var i = DepartmentInfo.GetForId(120001);
                var m = DepartmentInfo.GetForId(130001);

                var shenjia = p.Income + f.Income + i.Income + m.Income;

                await UpdateShenjia(sm, roleId, shenjia);///更新身价

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
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }


        /// <summary>
        /// 创建公司
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="role"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static async Task<CreateCompanyResult> CreateCompany(IReliableStateManager sm, UserRole role, string name, CreateCompanyResult result)
        {
            try
            {
                Company company = new Company(name);
                companyCollection = await sm.GetOrAddAsync<IReliableDictionary<Guid, Company>>(RoleIdBindCompanyName);
                using (var tx = sm.CreateTransaction())
                {
                    await companyCollection.AddAsync(tx, role.Id, company);
                    await tx.CommitAsync();
                }
                result.DepartmentInfo = await CreateDepartment(sm, role.Id);    //创建部门并初始化

                result.CompanyInfo.Id = company.Id.ToString();
                result.CompanyInfo.Level = company.Level;
                result.CompanyInfo.Name = company.Name;
                result.CompanyInfo.CurExp = company.CurExp;
                //result.CompanyInfo.CurPersonnelLv = company.CurPersonnelLv;
                //result.CompanyInfo.CurFinanceLv = company.CurFinanceLv;
                //result.CompanyInfo.CurMarketLv = company.CurMarketLv;
                //result.CompanyInfo.CurInvestmentLv = company.CurInvestmentLv;
                result.CompanyInfo.CreateTime = company.CreateTime.Date.ToString();
                return result;
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        internal static async Task<bool> CheckCompanyExists(IReliableStateManager sm, Guid role)
        {
            companyCollection = await sm.GetOrAddAsync<IReliableDictionary<Guid, Company>>(RoleIdBindCompanyName);
            using (var tx = sm.CreateTransaction())
            {
                if (await companyCollection.ContainsKeyAsync(tx, role))
                    return true;
                else
                    return false;
            }
        }

        internal static async Task<bool> CheckDepartmentExists(IReliableStateManager sm, Guid role)
        {
            roleIdBindDepartment = await sm.GetOrAddAsync<IReliableDictionary<Guid, DepartmentGroup>>(RoleIdBindDepartmentName);
            using (var tx = sm.CreateTransaction())
            {
                if (await roleIdBindDepartment.ContainsKeyAsync(tx, role))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 检查公司是否重名
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static async Task<bool> CheckCompanyName(IReliableStateManager sm, string name)
        {
            try
            {
                companyNameBindCompanyID = await sm.GetOrAddAsync<IReliableDictionary<string, Guid>>(CompanyNameBindCompanyIdName);
                using (var tx = sm.CreateTransaction())
                {
                    return await companyNameBindCompanyID.ContainsKeyAsync(tx, name);
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }


        /// <summary>
        /// 恢复角色属性到初始值状态
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>

        public static async Task ResetRoleAttrInfoByRoleIdAsync(IReliableStateManager sm, Guid roleId)
        {
            if (roleId == null) throw new ArgumentNullException();
            try
            {
                List<Model.Data.Npc.UserAttr> userAttr = new List<Model.Data.Npc.UserAttr>();
                roleIdBindUserAttrs = await sm.GetOrAddAsync<IReliableDictionary<Guid, List<Model.Data.Npc.UserAttr>>>(RoleIdBindUserAttrsName);
                using (var tx = sm.CreateTransaction())
                {
                    var role = await roleIdBindUserAttrs.TryGetValueAsync(tx, roleId);
                    if (role.HasValue)
                    {
                        await roleIdBindUserAttrs.TryUpdateAsync(tx, roleId, userAttr, null);
                    }
                    else
                    {
                        await roleIdBindUserAttrs.AddAsync(tx, roleId, userAttr);
                    }

                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }


        /// <summary>
        /// 获取用户属性信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static async Task<List<Model.Data.Npc.UserAttr>> GetRoleAttrAsync(IReliableStateManager sm, Guid roleId)
        {
            try
            {
                var role = await GetRoleInfoByRoleIdAsync(sm, roleId);
                return (role == null) ? null : role.UserAttr;
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }

        }

        /// <summary>
        /// 角色等级提升
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>

        public static async Task RoleLevelUpAsync(IReliableStateManager sm, Guid roleId)
        {
            var role = await GetRoleInfoByRoleIdAsync(sm, roleId);
            if (role != null)
            {
                var old = role;
                role.Level++;
                var levelAttr = Level.GetForLv(role.Level);
                if (levelAttr != null)
                {
                    role = AddRoleAttrByLevelUpAsync(sm, role, levelAttr);

                    await UpdateRoleInfoByRoleIdAsync(sm, roleId, role, old);
                    //通知客户端

                    //产生一个通知  通知下发给用户
                    Model.ResponseData.TCLevelUpResult result = new Model.ResponseData.TCLevelUpResult();
                    result.RoleAttr = new List<Model.ResponseData.UserAttr>();
                    result.NewExp = role.Exp;
                    result.NewLevel = role.Level;
                    foreach (var attr in role.UserAttr)
                    {
                        result.RoleAttr.Add(new Model.ResponseData.UserAttr()
                        {
                            Count = attr.Count,
                            UserAttrID = attr.UserAttrID
                        });
                    }
                    var data = await InitHelpers.GetPse().SerializeAsync(result);
                    await MsgMaker.SendMessage(WSResponseMsgID.TCLevelUpResult, 1, roleId, sm, data);    //写入队列
                }
            }
        }


        /// <summary>
        /// 当等级提升时，修改玩家属性
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="role"></param>
        /// <param name="levelAttr"></param>
        /// <returns></returns>
        private static UserRole AddRoleAttrByLevelUpAsync(IReliableStateManager sm, UserRole role, Level levelAttr)
        {
            for (int i = 1; i <= levelAttr.Attribute.Count; i++)
            {
                var pro = role.UserAttr.First(a => a.UserAttrID == i);
                pro.Count += levelAttr.Attribute.First(l => l.AttributeID == i).Count;
            }
            return role;
        }


        /// <summary>
        /// 获取用户等级信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static async Task<int> GetRoleLevelAsync(IReliableStateManager sm, Guid roleId)
        {
            try
            {
                var role = await GetRoleInfoByRoleIdAsync(sm, roleId);
                return (role == null) ? -1 : role.Level;
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }



        /// <summary>
        /// 初始化用户信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static async Task InitRoleInfo(IReliableStateManager sm, Guid roleId)
        {
            if (roleId == null) throw new ArgumentNullException();
            try
            {
                //  await InitRoleBaseInfoByRoleIdAsync(sm, roleId);
                await BindUserBagByRoleIdAsync(sm, roleId); //初始化背包
                //await ResetRoleAttrInfoByRoleIdAsync(sm, roleId);     //初始化 角色属性
                //TODO 其他初始化
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }


        /// <summary>
        /// 更新角色信息通过角色id
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task UpdateRoleInfoByRoleIdAsync(IReliableStateManager sm, Guid roleId, UserRole role, UserRole comp)
        {
            if (roleId == null) throw new ArgumentNullException();
            if (role == null) throw new ArgumentNullException();
            try
            {
                roleCollection = await sm.GetOrAddAsync<IReliableDictionary<Guid, UserRole>>(RoleName); //角色数据
                using (var tx = sm.CreateTransaction())
                {
                    await roleCollection.TryUpdateAsync(tx, roleId, role, comp);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO
                throw ex;
            }

        }



        /// <summary>
        /// 初始化用户基本属性
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private static async Task InitRoleBaseInfoByRoleIdAsync(IReliableStateManager sm, Guid roleId)
        {
            if (roleId == null) throw new ArgumentNullException();
            try
            {

                var role = await GetRoleInfoByRoleIdAsync(sm, roleId);
                if (role != null)
                {
                    Character u = Character.GetIndex(0);
                    UserRole user = role;
                    if (user.Sex == 1)
                    {
                        u = Character.GetForId(1);
                    }
                    else if (user.Sex == 2)
                    {
                        u = Character.GetForId(2);
                    }
                    user.Icon = u.Icon;
                    user.Level = u.Level;
                    user.Avatar = u.Avatar;
                    foreach (var attr in u.Attribute)
                    {
                        user.UserAttr.Add(new Model.Data.Npc.UserAttr()
                        {
                            Count = attr.Count,
                            UserAttrID = attr.AttributeID
                        });
                    }
                    await UpdateRoleInfoByRoleIdAsync(sm, roleId, user, role);
                }

            }
            catch (Exception ex)
            {
                //TODO
                throw ex;
            }

        }




        /// <summary>
        /// 通过roleid更新角色属性信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="userAttr"></param>
        /// <returns></returns>
        public static async Task UpdateUserAttrInfoByRoldIdAsync(IReliableStateManager sm, Guid roleId, List<Model.Data.Npc.UserAttr> userAttr)
        {
            if (roleId == null) throw new ArgumentNullException();
            if (userAttr == null || !userAttr.Any()) throw new ArgumentNullException();
            try
            {
                roleIdBindUserAttrs = await sm.GetOrAddAsync<IReliableDictionary<Guid, List<Model.Data.Npc.UserAttr>>>(RoleIdBindUserAttrsName);
                using (var tx = sm.CreateTransaction())
                {
                    await roleIdBindUserAttrs.TryUpdateAsync(tx, roleId, userAttr, null);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }


        /// <summary>
        /// 通过角色id绑定角色属性值
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="userAttr"></param>
        /// <returns></returns>
        public static async Task BindUserAttrInfoByRoleIdAsync(IReliableStateManager sm, Guid roleId, List<Model.Data.Npc.UserAttr> userAttr)
        {
            if (roleId == null) throw new ArgumentNullException();
            if (userAttr == null || !userAttr.Any()) throw new ArgumentNullException();
            try
            {
                roleIdBindUserAttrs = await sm.GetOrAddAsync<IReliableDictionary<Guid, List<Model.Data.Npc.UserAttr>>>(RoleIdBindUserAttrsName);
                using (var tx = sm.CreateTransaction())
                {
                    await roleIdBindUserAttrs.AddAsync(tx, roleId, userAttr);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }

        }


        /// <summary>
        /// 通过角色id查找session
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task<string> GetOnlineSessionByRoleIdAsync(IReliableStateManager sm, Guid roleId)
        {
            if (roleId == null) throw new ArgumentNullException();
            try
            {
                RoleIdBindSession = await sm.GetOrAddAsync<IReliableDictionary<Guid, string>>(RoleIdBindSessionName);
                using (var tx = sm.CreateTransaction())
                {
                    var session = await RoleIdBindSession.TryGetValueAsync(tx, roleId);
                    return session.HasValue ? session.Value : null;
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }


        /// <summary>
        /// 通过角色id 更新 session
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static async Task UpdateOnlineSessionByRoleIdAsync(IReliableStateManager sm, Guid roleId, string session, string cmpSid)
        {
            if (roleId == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(session)) throw new ArgumentNullException();
            try
            {
                RoleIdBindSession = await sm.GetOrAddAsync<IReliableDictionary<Guid, string>>(RoleIdBindSessionName);
                using (var tx = sm.CreateTransaction())
                {
                    await RoleIdBindSession.TryUpdateAsync(tx, roleId, session, cmpSid);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }




        /// <summary>
        /// 通过角色id 移除当前在线角色的session
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static async Task RemoveOnlineSessionByRoleIdAsync(IReliableStateManager sm, Guid roleId)
        {

            if (roleId == null) throw new ArgumentNullException();
            try
            {
                RoleIdBindSession = await sm.GetOrAddAsync<IReliableDictionary<Guid, string>>(RoleIdBindSessionName);
                using (var tx = sm.CreateTransaction())
                {
                    await RoleIdBindSession.TryRemoveAsync(tx, roleId);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过角色id绑定session
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static async Task BindOnlineSessionByRoleIdAsync(IReliableStateManager sm, Guid roleId, string session)
        {
            if (roleId == null) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(session)) throw new ArgumentNullException();
            try
            {
                RoleIdBindSession = await sm.GetOrAddAsync<IReliableDictionary<Guid, string>>(RoleIdBindSessionName);
                using (var tx = sm.CreateTransaction())
                {
                    await RoleIdBindSession.AddAsync(tx, roleId, session);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }



        /// <summary>
        /// 通过session查找当前在线的角色
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <returns>角色 或者 null</returns>
        //[Obsolete]
        public static async Task<UserRole> GetOnlineRoleBySessionAsync(IReliableStateManager sm, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException();

            try
            {
                sessionBindRole = await sm.GetOrAddAsync<IReliableDictionary<string, UserRole>>(SessionBindRoleIdName);
                using (var tx = sm.CreateTransaction())
                {
                    var role = await sessionBindRole.TryGetValueAsync(tx, sessionId);
                    return role.HasValue ? role.Value : null;
                }

            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }



        /// <summary>
        /// 通过session绑定当前在线角色
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        //[Obsolete]
        public static async Task BindOnlineRoleBySessionAsync(IReliableStateManager sm, string sessionId, UserRole role)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException();
            if (role == null) throw new ArgumentNullException();
            try
            {
                sessionBindRole = await sm.GetOrAddAsync<IReliableDictionary<string, UserRole>>(SessionBindRoleIdName);
                using (var tx = sm.CreateTransaction())
                {
                    await sessionBindRole.AddAsync(tx, sessionId, role);
                    await tx.CommitAsync();
                }

            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }

        }

        /// <summary>
        /// 通过session更新当前在线角色
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        //[Obsolete]
        public static async Task UpdateOnlineRoleBySessionAsync(IReliableStateManager sm, string sessionId, UserRole role)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException();
            if (role == null) throw new ArgumentNullException();
            try
            {
                sessionBindRole = await sm.GetOrAddAsync<IReliableDictionary<string, UserRole>>(SessionBindRoleIdName);
                using (var tx = sm.CreateTransaction())
                {
                    var onlineR = await sessionBindRole.TryGetValueAsync(tx, sessionId);
                    if (onlineR.HasValue)
                    {
                        await sessionBindRole.TryUpdateAsync(tx, sessionId, role, null);
                        await tx.CommitAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过session移除当前在线角色
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
       // [Obsolete]
        public static async Task RemoveOnlineRoleBySessionAsync(IReliableStateManager sm, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException();
            try
            {
                sessionBindRole = await sm.GetOrAddAsync<IReliableDictionary<string, UserRole>>(SessionBindRoleIdName);
                using (var tx = sm.CreateTransaction())
                {
                    var onlineR = await sessionBindRole.TryGetValueAsync(tx, sessionId);
                    if (onlineR.HasValue)
                    {
                        await sessionBindRole.TryRemoveAsync(tx, sessionId);
                        await tx.CommitAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过角色id获取角色属性值
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static async Task<List<Model.Data.Npc.UserAttr>> GetUserAttrInfoByRoleIdAsync(IReliableStateManager sm, Guid roleId)
        {
            if (roleId == null) throw new ArgumentNullException();
            try
            {
                roleIdBindUserAttrs = await sm.GetOrAddAsync<IReliableDictionary<Guid, List<Model.Data.Npc.UserAttr>>>(RoleIdBindUserAttrsName);
                using (var tx = sm.CreateTransaction())
                {
                    var userAttr = await roleIdBindUserAttrs.TryGetValueAsync(tx, roleId);
                    return userAttr.HasValue ? userAttr.Value : null;
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }


        #region 通过角色id查找角色数据 GetSingleRoleByRoleIdAsync(IReliableStateManager sm, Guid roleId)
        /// <summary>
        /// 通过角色id查找角色数据
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns>角色数据  或者 null</returns>
        public static async Task<UserRole> GetRoleInfoByRoleIdAsync(IReliableStateManager sm, Guid roleId)
        {
            #region 非空检查
            if (roleId == null) throw new ArgumentNullException(roleId.ToString());
            #endregion
            roleCollection = await sm.GetOrAddAsync<IReliableDictionary<Guid, UserRole>>(RoleName); //角色数据
            using (var tx = sm.CreateTransaction())
            {
                var user = await roleCollection.TryGetValueAsync(tx, roleId);
                return user.HasValue ? user.Value : null;
            }
        }
        #endregion


        #region 通过sessionid 拿到用户账号数据 GetAccountBySessionAsync(IReliableStateManager sm, string sessionId)
        /// <summary>
        /// 通过sessionId 拿到用户账号
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <returns>用户账号</returns>
        public static async Task<Account> GetAccountBySessionAsync(IReliableStateManager sm, string sessionId)
        {
            #region 非空验证
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException(sessionId);
            #endregion
            userNameSessionCollection = await sm.GetOrAddAsync<IReliableDictionary<string, string>>(UserNameSessionName);
            accountsCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsCollectionName);
            using (var tx = sm.CreateTransaction())
            {
                try
                {
                    //var token = await GetTokenBySessionIdAsync(sm, sessionId);
                    //if (token != null)
                    //{
                    //     var account = await accountsCollection.TryGetValueAsync(tx, token.UserPassport);
                    //    if (account.HasValue)   //在哪 账号唯一id
                    //    {
                    //        return account.Value;
                    //    }
                    //    else return null;
                    //}
                    var userName = await userNameSessionCollection.TryGetValueAsync(tx, sessionId);
                    if (userName.HasValue)  //先拿账号 user123456
                    {
                        var account = await accountsCollection.TryGetValueAsync(tx, userName.Value);
                        if (account.HasValue)   //在哪 账号唯一id
                        {
                            return account.Value;
                        }
                        else return null;
                    }
                    else return null;
                }
                catch (Exception ex)
                {
                    //TODO 日志处理
                    throw ex;
                }
            }
        }


        /// <summary>
        /// 检查登入的账号和通过session保存的账号是否相同
        /// </summary>
        /// <param name="act"></param>
        /// <param name="userPassport"></param>
        public static bool CheckPassport(string act, string userPassport)
        {
            return act.Equals(userPassport);
        }


        /// <summary>
        /// 检查session和上次保存的是否相同
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="sid"></param>
        /// <returns></returns>
        public static bool CheckSession(string sessionId, string sid)
        {
            return sid.Equals(sessionId);
        }
        #endregion

        #region 通过账号的唯一id拿到角色列表 GetRoleListsByAccountIdAsync(IReliableStateManager sm, Guid accountId)
        /// <summary>
        /// 通过账号的唯一id拿到角色列表
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="accountId"></param>
        /// <returns>List<UserRole></returns>
        public static async Task<List<UserRole>> GetRoleListsByAccountIdAsync(IReliableStateManager sm, Guid accountId)
        {
            #region 非空验证
            if (accountId == null) throw new ArgumentNullException(accountId.ToString());
            #endregion
            accountRoleCollection = await sm.GetOrAddAsync<IReliableDictionary<Guid, List<UserRole>>>(AccountRoleName);
            using (var tx = sm.CreateTransaction())
            {
                var account = await accountRoleCollection.TryGetValueAsync(tx, accountId);
                return account.HasValue ? account.Value : null;
            }
        }
        #endregion


        /// <summary>
        /// userName 绑定 session
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="userName"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static async Task BindSessionByUserName(IReliableStateManager sm, string userName, string session)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(userName);
            if (string.IsNullOrEmpty(session)) throw new ArgumentNullException(session);
            try
            {
                sessionUserNameCollection = await sm.GetOrAddAsync<IReliableDictionary<string, string>>(SessionCollectionName); //session数据
                using (var tx = sm.CreateTransaction())
                {
                    await sessionUserNameCollection.AddAsync(tx, userName, session);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {    //TODO 日志处理
                throw ex;
            }
        }

        /// <summary>
        /// userName 拿到 session
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <returns>角色名</returns>
        public static async Task<string> GetSessionByUserNameAsync(IReliableStateManager sm, string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(userName);
            try
            {
                sessionUserNameCollection = await sm.GetOrAddAsync<IReliableDictionary<string, string>>(SessionCollectionName); //session数据
                using (var tx = sm.CreateTransaction())
                {
                    var sessionId = await sessionUserNameCollection.TryGetValueAsync(tx, userName);
                    return sessionId.HasValue ? sessionId.Value : null;
                }
            }
            catch (Exception ex)
            {
                //TODO 日志处理
                throw ex;
            }
        }


        /// <summary>
        /// 通过username移除session
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task RemoveSessionByUserName(IReliableStateManager sm, string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(userName);
            try
            {
                sessionUserNameCollection = await sm.GetOrAddAsync<IReliableDictionary<string, string>>(SessionCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await sessionUserNameCollection.TryRemoveAsync(tx, userName);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过sessionId移除username
        /// </summary>
        /// <returns></returns>
        public static async Task RemoveUserNameBySession(IReliableStateManager sm, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException(sessionId);
            try
            {
                userNameSessionCollection = await sm.GetOrAddAsync<IReliableDictionary<string, string>>(UserNameSessionName);
                using (var tx = sm.CreateTransaction())
                {
                    await userNameSessionCollection.TryRemoveAsync(tx, sessionId);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }

        }

        /// <summary>
        /// 通过username更新session
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task UpdateSessionByUserNameAsync(IReliableStateManager sm, string userName, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(userName)) throw new ArgumentNullException();

            try
            {
                sessionUserNameCollection = await sm.GetOrAddAsync<IReliableDictionary<string, string>>(SessionCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await sessionUserNameCollection.TryUpdateAsync(tx, userName, sessionId, null);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过session更新username
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task UpdateUserNameBySessionAsync(IReliableStateManager sm, string sessionId, string userName)
        {
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(userName)) throw new ArgumentNullException();

            try
            {
                userNameSessionCollection = await sm.GetOrAddAsync<IReliableDictionary<string, string>>(UserNameSessionName);
                using (var tx = sm.CreateTransaction())
                {
                    await userNameSessionCollection.TryUpdateAsync(tx, sessionId, userName, null);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过session检查username是否存在
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task<bool> CheckUserNameSessionListBySessionAsync(IReliableStateManager sm, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException(sessionId);
            try
            {
                userNameSessionCollection = await sm.GetOrAddAsync<IReliableDictionary<string, string>>(UserNameSessionName);
                using (var tx = sm.CreateTransaction())
                {
                    return await userNameSessionCollection.ContainsKeyAsync(tx, sessionId);
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过username检查session是否存在
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task<bool> CheckSessionByUserNameAsync(IReliableStateManager sm, string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(userName);
            try
            {
                sessionUserNameCollection = await sm.GetOrAddAsync<IReliableDictionary<string, string>>(SessionCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    return await sessionUserNameCollection.ContainsKeyAsync(tx, userName);
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过session 拿到 userName
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task<string> GetUserNameBySessionAsync(IReliableStateManager sm, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException(sessionId);
            try
            {
                userNameSessionCollection = await sm.GetOrAddAsync<IReliableDictionary<string, string>>(UserNameSessionName);
                using (var tx = sm.CreateTransaction())
                {
                    var name = await userNameSessionCollection.TryGetValueAsync(tx, sessionId);
                    return name.HasValue ? name.Value : null;
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过session绑定 userName
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="session"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task BindUserNameBySessionAsync(IReliableStateManager sm, string session, string userName)
        {
            if (string.IsNullOrEmpty(session)) throw new ArgumentNullException(session);
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException(userName);
            try
            {
                userNameSessionCollection = await sm.GetOrAddAsync<IReliableDictionary<string, string>>(UserNameSessionName);
                using (var tx = sm.CreateTransaction())
                {
                    await userNameSessionCollection.AddAsync(tx, session, userName);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        #region 通过客户端传来的角色名 检查角色名是否重复 Task<bool> CheckRoleNameIsExistsByNameAsync(IReliableStateManager sm, string name)
        /// <summary>
        /// 通过客户端传来的角色名检查角色名是否重复
        /// </summary>
        /// <param name="stateManager"></param>
        /// <param name="name"></param>
        /// <returns>bool</returns>
        public static async Task<bool> CheckRoleNameIsExistsByNameAsync(IReliableStateManager sm, string name)
        {
            #region 非空检查
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(name);
            #endregion
            idRoleCollection = await sm.GetOrAddAsync<IReliableDictionary<string, IdRole>>(IdRoleName);
            try
            {
                using (var tx = sm.CreateTransaction())
                {
                    return await idRoleCollection.ContainsKeyAsync(tx, name, LockMode.Default);
                }
            }
            catch (Exception ex)
            {
                //TODO 日志处理
                throw ex;
            }

        }
        #endregion
        #region 绑定角色名和角色id  Task SetRoleNameBindRoleIdAsync(IReliableStateManager sm, string roleName, IdRole idRole)
        /// <summary>
        /// 绑定角色名和角色id  方便用来查询用户名是否重复等等
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleName"></param>
        /// <param name="idRole"></param>
        /// <returns></returns>
        public static async Task SetRoleNameBindRoleIdAsync(IReliableStateManager sm, string roleName, IdRole idRole)
        {
            #region 非空验证
            if (string.IsNullOrEmpty(roleName)) throw new ArgumentNullException(roleName);
            if (idRole == null) throw new ArgumentNullException(idRole.ToString());
            #endregion
            try
            {
                idRoleCollection = await sm.GetOrAddAsync<IReliableDictionary<string, IdRole>>(IdRoleName);
                using (var tx = sm.CreateTransaction())
                {
                    await idRoleCollection.AddAsync(tx, roleName, idRole);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }

        }
        #endregion
        #region 设置roleId为角色的key   Task SetRoleInfoByRoleIdAsync(IReliableStateManager sm, Guid roleId, UserRole newRole)
        /// <summary>
        /// 设置roleId为角色的key 
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId">key</param>
        /// <param name="newRole">value</param>
        /// <returns></returns>
        public static async Task BindRoleInfoByRoleIdAsync(IReliableStateManager sm, Guid roleId, UserRole newRole)
        {
            if (roleId == null) throw new ArgumentNullException();
            if (newRole == null) throw new ArgumentNullException();
            try
            {
                roleCollection = await sm.GetOrAddAsync<IReliableDictionary<Guid, UserRole>>(RoleName);
                using (var tx = sm.CreateTransaction())
                {
                    await roleCollection.AddAsync(tx, roleId, newRole);
                    await tx.CommitAsync();
                }
                //更新角色属性表
                //await ResetRoleAttrInfoByRoleIdAsync(sm, roleId);
                //更新角色背包属性
                await BindUserBagByRoleIdAsync(sm, roleId);
            }
            catch (Exception ex)
            {
                //TODO 日志记录

                throw ex;
            }

        }

        #endregion

        /// <summary>
        /// 通过角色id获取角色背包信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static async Task<Bag> GetRoleBagByRoleIdAsync(IReliableStateManager sm, Guid roleId)
        {
            if (roleId == null) throw new ArgumentNullException();
            try
            {
                roleIdBindBag = await sm.GetOrAddAsync<IReliableDictionary<Guid, Bag>>(RoleIdBindBagName);
                using (var tx = sm.CreateTransaction())
                {
                    var box = await roleIdBindBag.TryGetValueAsync(tx, roleId);
                    return box.HasValue ? box.Value : null;
                }
            }
            catch (Exception ex)
            {
                //TODO
                throw ex;
            }
        }


        /// <summary>
        /// 通过roleId 初始化玩家的背包属性
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static async Task<Bag> BindUserBagByRoleIdAsync(IReliableStateManager sm, Guid roleId)
        {
            if (roleId == null) throw new ArgumentNullException();
            try
            {
                Bag box = new Bag();
                roleIdBindBag = await sm.GetOrAddAsync<IReliableDictionary<Guid, Bag>>(RoleIdBindBagName);
                using (var tx = sm.CreateTransaction())
                {
                    await roleIdBindBag.AddAsync(tx, roleId, box);
                    await tx.CommitAsync();
                }
                return await GetRoleBagByRoleIdAsync(sm, roleId);
            }
            catch (Exception ex)
            {
                //TODO
                throw ex;
            }
        }


        /// <summary>
        /// 通过角色id更新背包信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="roleId"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static async Task UpdateRoleBagByRoleIdAsync(IReliableStateManager sm, Guid roleId, Bag newBg, Bag oldBg)
        {
            if (roleId == null) throw new ArgumentNullException();
            if (newBg == null) throw new ArgumentNullException();
            try
            {
                roleIdBindBag = await sm.GetOrAddAsync<IReliableDictionary<Guid, Bag>>(RoleIdBindBagName);
                using (var tx = sm.CreateTransaction())
                {
                    await roleIdBindBag.TryUpdateAsync(tx, roleId, newBg, oldBg);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO
                throw ex;
            }
        }


        /// <summary>
        /// 通过signtoken移除token
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="signToken"></param>
        /// <returns></returns>
        public static async Task RemoveTokenBysignToken(IReliableStateManager sm, string signToken)
        {
            if (string.IsNullOrEmpty(signToken)) throw new ArgumentNullException();
            try
            {
                tokenCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Token>>(TokenCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await tokenCollection.TryRemoveAsync(tx, signToken);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过signtoken更新token
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="signToken"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task UpdateTokenBySignTokenAsync(IReliableStateManager sm, string signToken, Token token)
        {
            if (string.IsNullOrEmpty(signToken)) throw new ArgumentNullException();
            if (token == null) throw new ArgumentNullException();
            try
            {
                tokenCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Token>>(TokenCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await tokenCollection.TryUpdateAsync(tx, signToken, token, null);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过signtoken绑定token
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="signToken"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task BindTokenBySignTokenAsync(IReliableStateManager sm, string signToken, Token token)
        {
            if (string.IsNullOrEmpty(signToken)) throw new ArgumentNullException();
            if (token == null) throw new ArgumentNullException();
            try
            {
                tokenCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Token>>(TokenCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await tokenCollection.AddAsync(tx, signToken, token);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过signtoken获取token
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="signToken"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static async Task<Token> GetTokenBySignTokenAsync(IReliableStateManager sm, string signToken)
        {
            if (string.IsNullOrEmpty(signToken)) throw new ArgumentNullException();
            try
            {
                tokenCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Token>>(TokenCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    var token = await tokenCollection.TryGetValueAsync(tx, signToken);
                    return token.HasValue ? token.Value : null;
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 移除token通过session
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task RemoveTokenBySessionAsync(IReliableStateManager sm, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException();
            try
            {
                tokenCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Token>>(TokenCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await tokenCollection.TryRemoveAsync(tx, sessionId);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过session更新token
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task UpdateTokenBySessionAsync(IReliableStateManager sm, string sessionId, Token token)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException();
            if (token == null) throw new ArgumentNullException();
            try
            {
                tokenCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Token>>(TokenCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await tokenCollection.TryUpdateAsync(tx, sessionId, token, null);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }


        /// <summary>
        /// 通过session绑定token
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task BindTokenBySessionAsync(IReliableStateManager sm, string sessionId, Token token)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException();
            if (token == null) throw new ArgumentNullException();
            try
            {
                tokenCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Token>>(TokenCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await tokenCollection.AddAsync(tx, sessionId, token);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过session取得token
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public static async Task<Token> GetTokenBySessionIdAsync(IReliableStateManager sm, string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException();
            try
            {
                tokenCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Token>>(TokenCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    var token = await tokenCollection.TryGetValueAsync(tx, sessionId);
                    return token.HasValue ? token.Value : null;
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }
        #region 更新账号下面的角色数据 UpdateRoleListsByAccountId(IReliableStateManager sm, Guid accountID, List<UserRole> roles)
        /// <summary>
        /// 更新账号下面的角色数据
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="accountID"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static async Task UpdateRoleListsByAccountId(IReliableStateManager sm, Guid accountID, List<UserRole> roles, bool addOrDec)
        {
            #region 参数验证
            if (accountID == null) throw new ArgumentNullException(accountID.ToString());
            if (roles.Count <= 0 || roles == null) throw new ArgumentNullException();
            #endregion
            try
            {
                accountRoleCollection = await sm.GetOrAddAsync<IReliableDictionary<Guid, List<UserRole>>>(AccountRoleName);
                using (var tx = sm.CreateTransaction())
                {
                    await accountRoleCollection.TryUpdateAsync(tx, accountID, roles, null);
                    await tx.CommitAsync();
                }
                await RoleNumberController(sm, accountID, addOrDec); //增加新角色了 需要给账号字段中的RoleNumber++
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }
        #endregion


        #region 设置账号id与账号绑定 Task BindAccountByAccountIdAsync(IReliableStateManager sm, Guid accountID, Account account)
        /// <summary>
        /// 设置账号id与账号绑定
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="accountID"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static async Task BindAccountByAccountIdAsync(IReliableStateManager sm, Guid accountID, Account account)
        {
            if (accountID == null) throw new ArgumentNullException();
            if (account == null) throw new ArgumentNullException();
            try
            {
                aIdBindAccount = await sm.GetOrAddAsync<IReliableDictionary<Guid, Account>>(AIdBindAccountName);
                using (var tx = sm.CreateTransaction())
                {
                    await aIdBindAccount.AddAsync(tx, accountID, account);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }
        #endregion
        #region 通过账号id获取账号 GetAccountByAccountIdAsync(IReliableStateManager sm, Guid accountID)
        /// <summary>
        /// 通过账号id获取账号
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public static async Task<Account> GetAccountByAccountIdAsync(IReliableStateManager sm, Guid accountID)
        {
            if (accountID == null) throw new ArgumentNullException();
            try
            {
                aIdBindAccount = await sm.GetOrAddAsync<IReliableDictionary<Guid, Account>>(AIdBindAccountName);
                using (var tx = sm.CreateTransaction())
                {
                    var account = await aIdBindAccount.TryGetValueAsync(tx, accountID);
                    return account.HasValue ? account.Value : null;
                }
            }
            catch (Exception ex)
            {//TODO  日志
                throw ex;
            }
        }
        #endregion


        #region 通过账号id更新账号数据 Task UpdateAccountByAccountIdAsync(IReliableStateManager sm, Guid accountID, Account accountNewData)
        /// <summary>
        /// 通过账号id更新账号数据
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="accountID"></param>
        /// <param name="accountNewData"></param>
        /// <returns></returns>
        public static async Task UpdateAccountByAccountIdAsync(IReliableStateManager sm, Guid accountID, Account accountNewData)
        {
            if (accountID == null) throw new ArgumentNullException();
            if (accountNewData == null) throw new ArgumentNullException();
            try
            {
                aIdBindAccount = await sm.GetOrAddAsync<IReliableDictionary<Guid, Account>>(AIdBindAccountName);
                using (var tx = sm.CreateTransaction())
                {
                    await aIdBindAccount.TryUpdateAsync(tx, accountID, accountNewData, null);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// 通过用户名更新账号信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="userName"></param>
        /// <param name="accountNewData"></param>
        /// <returns></returns>
        public static async Task UpdateAccountByUserNameAsync(IReliableStateManager sm, string userName, Account accountNewData)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException();
            if (accountNewData == null) throw new ArgumentNullException();
            try
            {
                accountsCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await accountsCollection.TryUpdateAsync(tx, userName, accountNewData, null);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }


        /// <summary>
        /// 通过用户名绑定账号
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="userName">key</param>
        /// <param name="account">value</param>
        /// <returns></returns>
        public static async Task BindAccountByUserNameAsync(IReliableStateManager sm, string userName, Account account)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException();
            if (account == null) throw new ArgumentNullException();
            try
            {
                accountsCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await accountsCollection.AddAsync(tx, userName, account);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 绑定用户名（或者imei）和登录信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="userName">可以是用户账号 也可以是 imei</param>
        /// <param name="login"></param>
        /// <returns></returns>
        public static async Task BindLoginInfoByUserNameAsync(IReliableStateManager sm, string userName, Login login)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException();
            if (null == login) throw new ArgumentNullException();
            try
            {
                loginCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Login>>(LoginCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await loginCollection.AddAsync(tx, userName, login);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过用户名获取登录信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task<Login> GetLoginInfoByUserNameAsync(IReliableStateManager sm, string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException();
            try
            {
                loginCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Login>>(LoginCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    var login = await loginCollection.TryGetValueAsync(tx, userName);
                    return login.HasValue ? login.Value : null;
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过用户名移除登录信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task RemoveLoginInfoByUserNameAsync(IReliableStateManager sm, string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException();
            try
            {
                loginCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Login>>(LoginCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await loginCollection.TryRemoveAsync(tx, userName);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过username更新登录信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="userName"></param>
        /// <param name="newLoginInfo"></param>
        /// <returns></returns>
        public static async Task UpdateLoginInfoByUserNameAsync(IReliableStateManager sm, string userName, Login newLoginInfo)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException();
            if (newLoginInfo == null) throw new ArgumentNullException();
            try
            {
                loginCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Login>>(LoginCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await loginCollection.TryUpdateAsync(tx, userName, newLoginInfo, null);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 绑定通行证信息 通过 imei
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="imei"></param>
        /// <param name="passport"></param>
        /// <returns></returns>
        public static async Task BindPassportByImeiAsync(IReliableStateManager sm, string imei, Passport passport)
        {
            if (string.IsNullOrEmpty(imei)) throw new ArgumentNullException();
            if (passport == null) throw new ArgumentNullException();
            try
            {
                passportCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Passport>>(PassportCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await passportCollection.AddAsync(tx, imei, passport);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 移除通行证信息通过 imei
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        public static async Task RemovePassportByImeiAsync(IReliableStateManager sm, string imei)
        {
            if (string.IsNullOrEmpty(imei)) throw new ArgumentNullException();
            try
            {
                passportCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Passport>>(PassportCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await passportCollection.TryRemoveAsync(tx, imei);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过imei 更新通行证信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="imei"></param>
        /// <param name="newPassport"></param>
        /// <returns></returns>
        public static async Task UpdatePassportByImeiAsync(IReliableStateManager sm, string imei, Passport newPassport)
        {
            if (string.IsNullOrEmpty(imei)) throw new ArgumentNullException();
            if (newPassport == null) throw new ArgumentNullException();
            try
            {
                passportCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Passport>>(PassportCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    await passportCollection.TryUpdateAsync(tx, imei, newPassport, null);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过imei获得passport信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="imei"></param>
        /// <returns></returns>
        public static async Task<Passport> GetPassportByImeiAsync(IReliableStateManager sm, string imei)
        {
            if (string.IsNullOrEmpty(imei)) throw new ArgumentNullException();
            try
            {
                passportCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Passport>>(PassportCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    var pass = await passportCollection.TryGetValueAsync(tx, imei);
                    return pass.HasValue ? pass.Value : null;
                }

            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        /// <summary>
        /// 通过用户名获取账号信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static async Task<Account> GetAccountByUserNameAsync(IReliableStateManager sm, string userName)
        {
            if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException();
            try
            {
                accountsCollection = await sm.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsCollectionName);
                using (var tx = sm.CreateTransaction())
                {
                    var account = await accountsCollection.TryGetValueAsync(tx, userName);
                    return account.HasValue ? account.Value : null;
                }
            }
            catch (Exception ex)
            {
                //TODO 日志
                throw ex;
            }
        }

        #region 添加新角色到指定账户中 Task AddNewRoleListsByAccountIdAsync(IReliableStateManager sm, Guid accountID, List<UserRole> roles)
        /// <summary>
        /// 添加新角色到指定账户中 创建新角色
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="accountID"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static async Task AddNewRoleListsByAccountIdAsync(IReliableStateManager sm, Guid accountID, List<UserRole> roles)
        {
            if (accountID == null) throw new ArgumentNullException();
            if (roles == null || !roles.Any()) throw new ArgumentNullException();

            try
            {
                aIdBindAccount = await sm.GetOrAddAsync<IReliableDictionary<Guid, Account>>(AIdBindAccountName);
                accountRoleCollection = await sm.GetOrAddAsync<IReliableDictionary<Guid, List<UserRole>>>(AccountRoleName);
                using (var tx = sm.CreateTransaction())
                {
                    await accountRoleCollection.TryAddAsync(tx, accountID, roles);
                    await tx.CommitAsync();
                }
                await RoleNumberController(sm, accountID, true); //增加新角色了 需要给账号字段中的RoleNumber++
            }
            catch (Exception ex)
            {
                //TODO 
                //日志
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// 账号字段中的roleNumber数值操作增加1
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public static async Task RoleNumberController(IReliableStateManager sm, Guid accountID, bool addOrDec)
        {
            var account = await GetAccountByAccountIdAsync(sm, accountID);
            if (addOrDec)
            {
                account.RoleNumber++;
            }
            else
            {
                account.RoleNumber--;
            }
            await UpdateAccountByAccountIdAsync(sm, accountID, account);
        }
    }
}
