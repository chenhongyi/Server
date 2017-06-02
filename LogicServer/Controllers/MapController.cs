using Microsoft.ServiceFabric.Data;
using Model.ResponseData;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Data.Business;
using LogicServer.Data;
using Model.RequestData;

namespace LogicServer.Controllers
{
    public class MapController : BaseInstance<MapController>
    {
        private const int MapWidth = 1000; //地图宽度
        private const int MapHeight = 1000;    //地图高度
        private const int MaxPosLength = 100;   //每次请求数据长度
        private const int MaxPosValue = 1001000;   //请求的参数最大值 超出这个数值属于伪造  x*1000+y的最大值 

        internal async Task<BaseResponseData> GetMapCell(IReliableStateManager sm, int[] pos, GetMapResult result)
        {
            //foreach (var item in pos)
            //{
            //    if (item > MaxPosValue)
            //    {
            //        result.Result = WsResult.ParamsError;
            //        return result;
            //    }
            //}
            if (pos.Length < 1)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var mapCells = await GetMapCell(sm, pos);
            if (mapCells == null)
            {
                result.Result = GameEnum.WsResult.NullMapCell;
                return result;
            }
            else
            {
                List<LoadShopInfo> shopList = new List<LoadShopInfo>();
                foreach (var m in mapCells)
                {
                    if (!string.IsNullOrEmpty(m.ShopId))
                    {
                        var shopInfo = await GetShopInfo(sm, m.ShopId);
                        if (shopInfo != null)
                        {
                            shopList.Add(shopInfo);
                        }
                    }
                }
                result.LoadMapInfoList = mapCells;
                result.LoadShopInfo = shopList;
            }
            return result;
        }

        public async Task<GetMapResult> GetRoleLandShopInfo(IReliableStateManager sm, Guid roleId)
        {
            GetMapResult result = new GetMapResult();
            result.LoadShopInfo = await GetShopListByRoleIdAsync(sm, roleId);
            result.LoadMapInfoList = await GetMapListByRoleIdAsync(sm, roleId);
            return result;

        }

        private async Task<List<LoadMapInfo>> GetMapListByRoleIdAsync(IReliableStateManager sm, Guid roleId)
        {
            var lands = await DataHelper.GetRoleLandsByPosAsync(sm, roleId);
            List<LoadMapInfo> list = new List<LoadMapInfo>();
            if (lands != null)
            {
                foreach (var l in lands)
                {
                    list.Add(new LoadMapInfo
                    {
                        PosX = l.PosX,
                        PoxY = l.PoxY,
                        RoleId = l.RoleId,
                        ShopId = l.ShopId,
                        State = l.State
                    });
                }
            }
            return list;
        }

        private async Task<List<LoadShopInfo>> GetShopListByRoleIdAsync(IReliableStateManager sm, Guid roleId)
        {
            var idList = await DataHelper.GetShopIdListByRoleIdAsync(sm, roleId);
            List<LoadShopInfo> list = new List<LoadShopInfo>();
            if (idList != null)
            {
                foreach (var id in idList)
                {
                    var shop = await DataHelper.GetShopProperty(sm, id);
                    if (shop != null)
                    {
                        list.Add(new LoadShopInfo()
                        {
                            Employee = shop.Employee,
                            GetMoney = shop.GetMoney,
                            Level = shop.Level,
                            Name = shop.Name,
                            Popularity = shop.Popularity,
                            ShopId = shop.Id,
                            ShopType = shop.ShopType,
                            Star = shop.Star,
                            TodayCanAdvartise = shop.TodayCanAdvartise
                        });
                    }
                }
            }
            return list;
        }

        private async Task<LoadShopInfo> GetShopInfo(IReliableStateManager sm, string shopId)
        {
            var shop = await DataHelper.GetShopProperty(sm, shopId);
            if (shop != null)
            {
                LoadShopInfo info = new LoadShopInfo()
                {
                    Employee = shop.Employee,
                    GetMoney = shop.GetMoney,
                    Level = shop.Level,
                    Name = shop.Name,
                    Popularity = shop.Popularity,
                    ShopId = shop.Id,
                    ShopType = shop.ShopType,
                    Star = shop.Star,
                    TodayCanAdvartise = shop.TodayCanAdvartise
                };
                return info;
            }
            return null;
        }

        /// <summary>
        /// 获取当前坐标土地信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private async Task<LandProperty> GetMapCell(IReliableStateManager sm, int pos)
        {
            return await DataHelper.GetMapProperty(sm, pos);
        }

        private async Task<List<LoadMapInfo>> GetMapCell(IReliableStateManager sm, int[] pos)
        {
            var cells = await DataHelper.GetMapPropertys(sm, pos);
            if (cells != null && cells.Any())
            {
                List<LoadMapInfo> list = new List<LoadMapInfo>();
                foreach (var i in cells)
                {
                    list.Add(new LoadMapInfo()
                    {
                        PosX = i.PosX,
                        PoxY = i.PoxY,
                        State = i.State,
                        RoleId = i.RoleId
                    });
                }
                return list;
            }
            return null;
        }

        internal async Task<BaseResponseData> BuyLand(IReliableStateManager sm, int pos, Guid roleId, BuyLandResult result)
        {
            if (pos > MaxPosValue)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            //获取土地状态
            var land = await DataHelper.GetLandInfo(sm, pos);

            if (land == null || land.State == (int)GameEnum.MapStatus.Empty)
            {//可以购买
                result = await BuyLand(sm, pos, roleId);
                return result;
            }
            else if (land.State == (int)GameEnum.MapStatus.Saled)
            {
                result.Result = GameEnum.WsResult.LandIsSaled;
                return result;
            }
            else
            {
                //TODO 其他状态尚未处理
                return result;
            }
        }

        private async Task<BuyLandResult> BuyLand(IReliableStateManager sm, int pos, Guid roleId)
        {
            BuyLandResult result = new BuyLandResult();
            var data = await DataHelper.BindLandByPosAsync(sm, pos, roleId);
            if (data != null)
            {

                result.PosX = data.PosX;
                result.PoxY = data.PoxY;
                result.State = data.State;
                result.RoleId = data.RoleId;

                return result;
            }
            result.Result = GameEnum.WsResult.BuyLandErr;
            return result;
        }

        internal async Task<BaseResponseData> CreateShop(IReliableStateManager sm, CreateBuildReq data, Guid roleId, CreateBuildResult result)
        {
            if (string.IsNullOrEmpty(data.Name) || data.Pos < 0 || data.Pos > MaxPosValue || data.ShopType < 0)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            //检查土地状态 是否可以购买
            var mapInfo = await GetMapCell(sm, data.Pos);
            if (mapInfo.State == (int)GameEnum.MapStatus.Saled && mapInfo.RoleId.Equals(roleId.ToString()))
            {
                result = await CreateShop(sm, data, roleId);
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
            else if (mapInfo.State == (int)GameEnum.MapStatus.Saled && !mapInfo.RoleId.Equals(roleId.ToString()))
            {
                result.Result = GameEnum.WsResult.NotYourLand;
            }
            return result;
        }

        private async Task<CreateBuildResult> CreateShop(IReliableStateManager sm, CreateBuildReq data, Guid roleId)
        {
            CreateBuildResult result = new CreateBuildResult();
            ShopProperty shop = new ShopProperty(data.Name, data.ShopType);
            await DataHelper.CreateShop(sm, data.Pos, shop, roleId);
            //TODO 更新身价
            //await DataHelper.UpdateShenjia(sm,roleId);
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
    }
}
