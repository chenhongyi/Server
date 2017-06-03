using LogicServer.Data;
using LogicServer.Data.Helper;
using Model;
using Model.Data.Business;
using Model.Protocol;
using Model.RequestData;
using Model.ResponseData;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Controllers
{
    public class BuildController : BaseInstance<BuildController>
    {
        private const int MapWidth = 1000; //地图宽度
        private const int MapHeight = 1000;    //地图高度
        private const int MaxPosLength = 100;   //每次请求数据长度
        private const int MaxPosValue = 1001000;   //请求的参数最大值 超出这个数值属于伪造  x*1000+y的最大值 

        public async Task CreateBuild(Guid roleId, int pos, BuildData build, int moneyCount, int moneyType)
        {
            try
            {
                FinanceLogData log = new FinanceLogData()
                {
                    AorD = false,
                    Count = -moneyCount,
                    Type = (int)GameEnum.FinanceLog.CreateBuild,
                    MoneyType = moneyType
                };
                LandData land = await LandDataHelper.Instance.GetLandByPos(pos);
                if (land != null)
                {
                    land.ShopId = build.Id;
                    land.State = (int)GameEnum.MapStatus.Building;
                }
                using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                {
                    await BagDataHelper.Instance.DecGold(moneyCount, moneyType, tx);    //扣钱
                    await BuildIdDataHelper.Instance.UpdateBuildIdListByRoleId(roleId, build.Id, tx);
                    await BuildDataHelper.Instance.SetBuildByBuildId(build, tx);
                    await BuildIdPosDataHelper.Instance.SetBuildIdByPos(pos, build.Id, tx);
                    await RoleController.Instance.AddIncome(roleId, build.Income, tx);
                    await LandDataHelper.Instance.UpdateLandByPos(pos, land, tx);
                    await FinanceLogController.Instance.UpdateFinanceLog(roleId, log, tx);
                    await tx.CommitAsync();
                }
                await MsgSender.Instance.UpdateGold(moneyType); //更新货币
                await MsgSender.Instance.UpdateIncome();        //更新身价
                await MsgSender.Instance.FinanceLogUpdate(log);
            }
            catch (Exception ex)
            {
                //Log
                throw ex;
            }
        }

        public async Task<CreateBuildResult> CreateBuild(Guid roleId, CreateBuildReq data)
        {
            CreateBuildResult result = new CreateBuildResult();
            BuildData build = new BuildData(roleId, data.Name, data.ShopType, data.Pos);
            var config = AutoData.Building.GetForId(data.ShopType);
            build.CostGold += config.BuildingCost.Count; //记录创建时消耗的金砖
            await CreateBuild(roleId, data.Pos, build, config.BuildingCost.Count, config.BuildingCost.GoldType);    //建造商铺
            result.LandBuildInfo.Employee = build.Employee;
            result.LandBuildInfo.GetMoney = build.GetMoney;
            result.LandBuildInfo.BuildId = build.Id;
            result.LandBuildInfo.Level = build.Level;
            result.LandBuildInfo.Name = build.Name;
            result.LandBuildInfo.Popularity = build.Popularity;
            result.LandBuildInfo.BuildType = build.BuildType;
            result.LandBuildInfo.Star = build.Star;
            result.LandBuildInfo.TodayCanAdvartise = build.TodayCanAdvartise;
            result.LandBuildInfo.Pos = build.Pos;
            return result;
        }

        internal async Task<BaseResponseData> CreateBuild(CreateBuildReq data, CreateBuildResult result)
        {
            if (string.IsNullOrEmpty(data.Name) || data.Pos < 0 || data.Pos > MaxPosValue || data.ShopType < 0)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var role = LogicServer.User.role;
            //检查土地状态 是否可以购买
            var mapInfo = await LandController.Instance.GetLandCell(data.Pos);
            if (mapInfo.State == (int)GameEnum.MapStatus.Saled && mapInfo.RoleId.Equals(LogicServer.User.role.Id.ToString()))
            {
                result = await CreateBuild(role.Id, data);
            }
            else if (mapInfo.State == (int)GameEnum.MapStatus.Building)
            {
                result.Result = GameEnum.WsResult.UnNullLand;
            }
            else if (mapInfo.State == (int)GameEnum.MapStatus.Empty)
            {
                result.Result = GameEnum.WsResult.UnOwnLand;
            }
            else if (mapInfo.State == (int)GameEnum.MapStatus.BanningLand)
            {
                result.Result = GameEnum.WsResult.BanningLand;
            }
            else if (mapInfo.State == (int)GameEnum.MapStatus.BanningBuild)
            {
                result.Result = GameEnum.WsResult.BanningBuild;
            }
            else if (mapInfo.State == (int)GameEnum.MapStatus.Saled && !mapInfo.RoleId.Equals(LogicServer.User.role.Id.ToString()))
            {
                result.Result = GameEnum.WsResult.NotYourLand;
            }
            return result;
        }


        public async Task<List<LoadBuildInfo>> GetBuildListByRoleIdAsync(Guid roleId)
        {
            var idList = await BuildIdDataHelper.Instance.GetBuildIdListByRoleId(roleId);
            List<LoadBuildInfo> list = new List<LoadBuildInfo>();
            if (idList != null)
            {
                foreach (var id in idList)
                {
                    var build = await BuildDataHelper.Instance.GetBuildByBuildId(id);
                    if (build != null)
                    {
                        list.Add(new LoadBuildInfo()
                        {
                            Employee = build.Employee,
                            GetMoney = build.GetMoney,
                            Level = build.Level,
                            Name = build.Name,
                            Popularity = build.Popularity,
                            BuildId = build.Id,
                            BuildType = build.BuildType,
                            Star = build.Star,
                            TodayCanAdvartise = build.TodayCanAdvartise,
                            Pos = build.Pos,
                            RoleId = build.RoleId
                        });
                    }
                }
            }
            return list;
        }

        public async Task<LoadBuildInfo> GetBuildInfo(string buildId)
        {
            var build = await BuildDataHelper.Instance.GetBuildByBuildId(buildId);
            if (build != null)
            {
                LoadBuildInfo info = new LoadBuildInfo()
                {
                    Employee = build.Employee,
                    GetMoney = build.GetMoney,
                    Level = build.Level,
                    Name = build.Name,
                    Popularity = build.Popularity,
                    BuildId = build.Id,
                    BuildType = build.BuildType,
                    Star = build.Star,
                    TodayCanAdvartise = build.TodayCanAdvartise,
                    Pos = build.Pos,
                    RoleId = build.RoleId
                };
                return info;
            }
            return null;
        }


        public async Task<BaseResponseData> DestoryBuild(string buildId, DestoryBuildResult result)
        {
            //检查店铺是否属于自己
            var roleId = LogicServer.User.role.Id;
            //var roleBuildList = await BuildIdDataHelper.Instance.GetBuildIdListByRoleId(roleId);
            //if (roleBuildList != null)
            //{
            //    if (roleBuildList.Contains(buildId))
            //    {
            var build = await BuildDataHelper.Instance.GetBuildByBuildId(buildId);

            if (build != null) //拿到店铺
            {
                if (build.RoleId.Equals(roleId.ToString()))
                {
                    var config = AutoData.BuildingLevel.GetForId(build.Level);
                    var goldType = config.DismantleCost.CurrencyID;
                    var goldCount = config.DismantleCost.Count;
                    var land = await LandController.Instance.GetLandCell(build.Pos);
                    FinanceLogData log = new FinanceLogData()
                    {
                        Count = goldCount,
                        MoneyType = goldType,
                        Type = (int)GameEnum.FinanceLog.DestoryBuild,
                        AorD = false

                    };
                    FinanceLogData log1 = new FinanceLogData()
                    {
                        Count = build.CostGold,
                        MoneyType = (int)GameEnum.Currency.Gold,
                        Type = (int)GameEnum.FinanceLog.DestoryBuild
                    };


                    using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                    {
                        await BagDataHelper.Instance.DecGold(goldCount, goldType, tx);                      //扣除货币
                        await BagDataHelper.Instance.AddGold(build.CostGold, (int)GameEnum.Currency.Gold, tx);  //返还消耗掉的金砖
                        await RoleController.Instance.AddIncome(roleId, build.Income, tx);                      //更新身价

                        {//销毁店铺
                            await BuildDataHelper.Instance.RemoveBuildByBuildId(buildId, tx);
                            await BuildIdDataHelper.Instance.RemoveOneBuildIdByRoleId(roleId, buildId, tx);
                            await BuildIdPosDataHelper.Instance.RemoveBuildIdByPos(build.Pos, tx);
                        }

                        { //更新土地状态
                            if (land != null)
                            {
                                land.ShopId = string.Empty;
                                land.State = (int)GameEnum.MapStatus.Saled;
                            }
                            await LandController.Instance.UpdateLandCell(build.Pos, land, tx);
                        }
                        await FinanceLogController.Instance.UpdateFinanceLog(roleId, log, tx);
                        await FinanceLogController.Instance.UpdateFinanceLog(roleId, log1, tx);
                        await tx.CommitAsync();
                    }

                    await MsgSender.Instance.UpdateIncome();
                    await MsgSender.Instance.UpdateGold(goldType);
                    await MsgSender.Instance.UpdateGold((int)GameEnum.Currency.Gold);
                    await MsgSender.Instance.FinanceLogUpdate(log);
                    await MsgSender.Instance.FinanceLogUpdate(log1);

                    result.LandInfo.BuildId = build.Id;
                    result.LandInfo.PosX = land.PosX;
                    result.LandInfo.PoxY = land.PoxY;
                    result.LandInfo.RoleId = land.RoleId;
                    result.LandInfo.State = land.State;
                    build = null;
                    return result;
                }
                else
                {
                    result.Result = GameEnum.WsResult.BuildIsNotExists;
                    return result;
                }
            }
            else
            {
                result.Result = GameEnum.WsResult.NoneBuilds;
                return result;
            }

            //    else
            //    {
            //        result.Result = GameEnum.WsResult.BuildIdNotOwnRole;
            //        return result;
            //    }
            //}

        }

        /// <summary>
        /// 店铺扩建
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponseData> OnBuildExtend()
        {
            BuildExtendResult result = new BuildExtendResult();
            if (LogicServer.User.bytes == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<BuildExtendReq>(LogicServer.User.bytes);
            if (data == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }

            return await BuildExtend(data.BuildId, result);
        }



        private async Task<LoadBuildInfo> BuildExtend(string buildId)
        {
            BuildExtendFailedResult result = new BuildExtendFailedResult();
            var roleId = LogicServer.User.role.Id;
            var build = await BuildDataHelper.Instance.GetBuildByBuildId(buildId);
            if (build != null)
            {
                if (!build.RoleId.Equals(roleId.ToString()))
                {
                    result.Result = GameEnum.WsResult.BuildIdNotOwnRole;
                    result.BuildId = buildId;
                    await MsgSender.Instance.BuildExtendFailed(result);
                    return null;
                }
                var department = await DepartmentGroupDataHelper.Instance.GetDepartMentGroupByRoleId(roleId);   //部门
                var curExtendLevel = department.Investment.CurExtension;    //当前扩建星级
                var config = AutoData.Extension.GetForId(curExtendLevel);
                if (config == null)
                {
                    result.Result = GameEnum.WsResult.ConfigErr;
                    result.BuildId = buildId;
                    await MsgSender.Instance.BuildExtendFailed(result);
                    return null;
                }
                if (!BagController.Instance.CheckMoney(config.UpgradeCost.Count, config.UpgradeCost.CurrencyID))
                {
                    result.Result = GameEnum.WsResult.NotEnoughMoney;
                    result.BuildId = buildId;
                    await MsgSender.Instance.BuildExtendFailed(result);
                    return null;
                }
                if (build.Level < config.NeedLv)
                {
                    result.Result = GameEnum.WsResult.NeedLevel;
                    result.BuildId = buildId;
                    await MsgSender.Instance.BuildExtendFailed(result);
                    return null;
                }
                var info = await BuildExtend(build, config, department);
                return info;
            }
            return null;
        }

        /// <summary>
        /// 扩建店铺 成功 构造返回
        /// </summary>
        /// <param name="build"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private async Task<LoadBuildInfo> BuildExtend(BuildData build, AutoData.Extension config, DepartmentGroup department)
        {
            var role = LogicServer.User.role;

            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                department.Investment.CurExtension++;   //扩建等级提升
                build.Employee += config.ClerkAddtion;
                build.CustomerAddtion += config.CustomerAddtion;
                build.Star++;
                build.Income += config.Income;
                role.SocialStatus += config.Income;
                await BagDataHelper.Instance.DecGold(config.UpgradeCost.Count, config.UpgradeCost.CurrencyID, tx);  //扣钱
                await RoleDataHelper.Instance.UpdateRoleByRoleIdAsync(role.Id, role, tx);                           //更新用户身价
                await BuildDataHelper.Instance.UpdateBuildByBuildId(build, tx);                                        //更新建筑
                await DepartmentGroupDataHelper.Instance.UpdateDepartMentGroupByRoleId(role.Id, department, tx);        //更新部门
                await tx.CommitAsync();
            }
            await MsgSender.Instance.UpdateGold(config.UpgradeCost.CurrencyID);
            await MsgSender.Instance.UpdateIncome();
            await MsgSender.Instance.UpdateDepartmentInvestment(department.Investment);
            LoadBuildInfo info = new LoadBuildInfo()
            {
                BuildId = build.Id,
                BuildType = build.BuildType,
                Employee = build.Employee,
                GetMoney = build.GetMoney,
                Level = build.Level,
                Name = build.Name,
                Popularity = build.Popularity,
                Pos = build.Pos,
                RoleId = build.RoleId,
                Star = build.Star,
                TodayCanAdvartise = build.TodayCanAdvartise,
                CustomerAddtion = build.CustomerAddtion
            };
            return info;
        }

        /// <summary>
        /// 店铺升级
        /// </summary>
        /// <returns></returns>
        public async Task<BaseResponseData> OnBuildLvUp()
        {
            BuildLvUpResult result = new BuildLvUpResult();
            if (LogicServer.User.bytes == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var data = await InitHelpers.GetPse().DeserializeAsync<BuildLvUpReq>(LogicServer.User.bytes);
            if (data == null)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            return await BuildLvUp(data.BuildId, result);
        }

        /// <summary>
        /// 检查条件
        /// </summary>
        /// <param name="buildId"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<BaseResponseData> BuildExtend(List<string> buildId, BuildExtendResult result)
        {
            foreach (var b in buildId)
            {
                var ret = await BuildExtend(b);
                if (ret != null)
                {
                    result.BuildInfo.Add(ret);
                }
            }
            return result;
        }


        /// <summary>
        /// 店铺升级 检查条件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<BuildLvUpResult> BuildLvUp(List<string> id, BuildLvUpResult result)
        {
            foreach (var i in id)
            {
                var ret = await BuildLvUp(i);
                if (ret != null)
                {
                    result.LandBuildInfo.Add(ret);
                }
            }
            return result;
        }

        private async Task<LoadBuildInfo> BuildLvUp(string id)
        {
            var role = LogicServer.User.role;
            BuildLvUpFailedResult failed = new BuildLvUpFailedResult();
            failed.BuildId = id;
            var build = await BuildDataHelper.Instance.GetBuildByBuildId(id);
            if (build == null)
            {
                failed.Result = GameEnum.WsResult.BuildIsNotExists;
                await MsgSender.Instance.BuildLvUpFailed(failed);
                return null;
            }
            if (!build.RoleId.Equals(role.Id.ToString()))
            {
                failed.Result = GameEnum.WsResult.BuildIdNotOwnRole;
                await MsgSender.Instance.BuildLvUpFailed(failed);
                return null;
            }
            var config = AutoData.BuildingLevel.GetForId(build.Level);
            if (config == null)
            {
                failed.Result = GameEnum.WsResult.ConfigErr;
                await MsgSender.Instance.BuildLvUpFailed(failed);
                return null;
            }
            var bag = LogicServer.User.bag;
            if (!BagController.Instance.CheckMoney(config.UpgradeCost.Count, config.UpgradeCost.CurrencyID))
            {
                failed.Result = GameEnum.WsResult.NotEnoughMoney;
                await MsgSender.Instance.BuildLvUpFailed(failed);
                return null;
            }
            var department = await DepartmentGroupDataHelper.Instance.GetDepartMentGroupByRoleId(role.Id);
            if (department == null)
            {
                failed.Result = GameEnum.WsResult.DepartmentInvalid;
                await MsgSender.Instance.BuildLvUpFailed(failed);
                return null;
            }
            var extendConfig = AutoData.Extension.GetForId(department.Investment.CurExtension + 1); //取扩建等级下一级的 needlv的值 作为升级上限
            if (build.Level + 1 > extendConfig.NeedLv)
            {
                failed.Result = GameEnum.WsResult.NeedExtendLevel;
                await MsgSender.Instance.BuildLvUpFailed(failed);
                return null;
            }
            build.CustomerAddtion += config.CustomerAddtion;
            build.Employee += config.ClerkNums;
            build.Popularity += config.Popularity;
            build.Income += config.Income;
            build.Level++;
            role.SocialStatus += config.Income;

            //升级
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await BagDataHelper.Instance.DecGold(config.UpgradeCost.Count, config.UpgradeCost.CurrencyID, tx);//扣钱
                await BuildDataHelper.Instance.UpdateBuildByBuildId(build, tx); //更新店铺
                await RoleDataHelper.Instance.UpdateRoleByRoleIdAsync(role.Id, role, tx);    //更新身价
                await tx.CommitAsync();
            }
            await MsgSender.Instance.UpdateIncome();
            await MsgSender.Instance.UpdateGold(config.UpgradeCost.CurrencyID);
            LoadBuildInfo info = new LoadBuildInfo()
            {
                BuildId = build.Id,
                BuildType = build.BuildType,
                CustomerAddtion = build.CustomerAddtion,
                Employee = build.Employee,
                GetMoney = build.GetMoney,
                Level = build.Level,
                Name = build.Name,
                Popularity = build.Popularity,
                Pos = build.Pos,
                RoleId = build.RoleId,
                Star = build.Star,
                TodayCanAdvartise = build.TodayCanAdvartise
            };
            return info;
        }
    }
}
