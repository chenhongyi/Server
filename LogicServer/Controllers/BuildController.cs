using LogicServer.Data;
using LogicServer.Data.Helper;
using Model;
using Model.Data.Business;
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
                using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                {
                    await BagDataHelper.Instance.DecGold(moneyCount, moneyType, tx);    //扣钱
                    await BuildIdDataHelper.Instance.UpdateBuildIdListByRoleId(roleId, build.Id, tx);
                    await BuildDataHelper.Instance.SetBuildByBuildId(build, tx);
                    await BuildIdPosDataHelper.Instance.SetBuildIdByPos(pos, build.Id, tx);
                    await RoleController.Instance.AddIncome(roleId, build.Income, tx);
                    await FinanceLogController.Instance.UpdateFinanceLog(roleId, log, tx);
                    await tx.CommitAsync();
                }
                await MsgSender.Instance.GoldUpdate(moneyType); //更新货币
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
            BuildData shop = new BuildData(data.Name, data.ShopType, data.Pos);
            var config = AutoData.Building.GetForId(data.ShopType);
            shop.CostGold += config.BuildingCost.Count; //记录创建时消耗的金砖
            await CreateBuild(roleId, data.Pos, shop, config.BuildingCost.Count, config.BuildingCost.GoldType);    //建造商铺




            result.Employee = shop.Employee;
            result.GetMoney = shop.GetMoney;
            result.Id = shop.Id;
            result.Level = shop.Level;
            result.Name = shop.Name;
            result.Popularity = shop.Popularity;
            result.ShopType = shop.ShopType;
            result.Star = shop.Star;
            result.TodayCanAdvartise = shop.TodayCanAdvartise;
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
                            ShopId = build.Id,
                            ShopType = build.ShopType,
                            Star = build.Star,
                            TodayCanAdvartise = build.TodayCanAdvartise,
                            Pos = build.Pos

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
                    ShopId = build.Id,
                    ShopType = build.ShopType,
                    Star = build.Star,
                    TodayCanAdvartise = build.TodayCanAdvartise,
                    Pos = build.Pos
                };
                return info;
            }
            return null;
        }


        public async Task<BaseResponseData> DestoryBuild(string buildId, DestoryBuildResult result)
        {
            //检查店铺是否属于自己
            var roleId = LogicServer.User.role.Id;
            var roleBuildList = await BuildIdDataHelper.Instance.GetBuildIdListByRoleId(roleId);
            if (roleBuildList != null)
            {
                if (roleBuildList.Contains(buildId))
                {
                    var build = await BuildDataHelper.Instance.GetBuildByBuildId(buildId);

                    if (build != null) //拿到店铺
                    {
#if DEBUG
                        var goldType = 1;
                        var goldCount = 1000;
#endif
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
                                roleBuildList.Remove(buildId);
                                await BuildIdDataHelper.Instance.UpdateBuildIdListByRoleId(roleId, roleBuildList, tx);
                                await BuildIdPosDataHelper.Instance.RemoveBuildIdByPos(build.Pos, tx);
                            }

                            { //更新土地状态
                                if (land != null)
                                {
                                    land.ShopId = string.Empty;
                                    land.State = (int)GameEnum.MapStatus.Empty;
                                }
                                await LandController.Instance.UpdateLandCell(build.Pos, land, tx);
                            }
                            await FinanceLogController.Instance.UpdateFinanceLog(roleId, log, tx);
                            await FinanceLogController.Instance.UpdateFinanceLog(roleId, log1, tx);
                            await tx.CommitAsync();
                        }
                        build = null;
                        await MsgSender.Instance.UpdateIncome();
                        await MsgSender.Instance.GoldUpdate(goldType);
                        await MsgSender.Instance.GoldUpdate((int)GameEnum.Currency.Gold);
                        await MsgSender.Instance.FinanceLogUpdate(log);
                        await MsgSender.Instance.FinanceLogUpdate(log1);

                        result.LandInfo.BuildId = string.Empty;
                        result.LandInfo.PosX = land.PosX;
                        result.LandInfo.PoxY = land.PoxY;
                        result.LandInfo.RoleId = land.RoleId;
                        result.LandInfo.State = land.State;
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
                    result.Result = GameEnum.WsResult.BuildIdNotOwnRole;
                    return result;
                }
            }
            else
            {
                result.Result = GameEnum.WsResult.NoneBuilds;
                return result;
            }
        }

    }
}
