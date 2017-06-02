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
using AutoData;
using GameEnum;
using LogicServer.Data.Helper;

namespace LogicServer.Controllers
{
    public class LandController : BaseInstance<LandController>
    {
        private const int MapWidth = 1000; //地图宽度
        private const int MapHeight = 1000;    //地图高度
        private const int MaxPosLength = 100;   //每次请求数据长度
        private const int MaxPosValue = 1001000;   //请求的参数最大值 超出这个数值属于伪造  x*1000+y的最大值 

        internal async Task<BaseResponseData> GetLandCell(int[] pos, GetMapResult result)
        {
            if (pos.Length < 1)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            var mapCells = await GetLandCell(pos);
            if (mapCells == null)
            {
                return result;
            }
            else
            {
                List<LoadBuildInfo> shopList = new List<LoadBuildInfo>();
                foreach (var m in mapCells)
                {
                    if (!string.IsNullOrEmpty(m.Value.BuildId))
                    {
                        var shopInfo = await BuildController.Instance.GetBuildInfo(m.Value.BuildId);
                        if (shopInfo != null)
                        {
                            shopList.Add(shopInfo);
                        }
                    }
                }
                result.LoadLandInfoDic = mapCells;
                result.LoadBuildInfo = shopList;
            }
            return result;
        }

        public async Task<GetMapResult> GetRoleLandShopInfo(Guid roleId)
        {

            GetMapResult result = new GetMapResult();

            result.LoadBuildInfo = await BuildController.Instance.GetBuildListByRoleIdAsync(roleId);
            result.LoadLandInfoDic = await GetLandListByRoleIdAsync(roleId);
            return result;

        }

        public async Task<Dictionary<int, LoadLandInfo>> GetLandListByRoleIdAsync(Guid roleId)
        {
            var lands = await LandDicDataHelper.Instance.GetLandDicByRoleIdAsync(roleId);
            Dictionary<int, LoadLandInfo> list = new Dictionary<int, LoadLandInfo>();
            if (lands != null)
            {
                foreach (var l in lands)
                {
                    list.Add(l.Key, new LoadLandInfo
                    {
                        PosX = l.Value.PosX,
                        PoxY = l.Value.PoxY,
                        RoleId = l.Value.RoleId,
                        BuildId = l.Value.ShopId,
                        State = l.Value.State
                    });
                }
            }
            return list;
        }




        /// <summary>
        /// 获取当前坐标土地信息
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public async Task<LandData> GetLandCell(int pos)
        {
            return await LandDataHelper.Instance.GetLandByPos(pos);
        }

        public async Task UpdateLandCell(int pos, LandData land, ITransaction tx)
        {
            await LandDataHelper.Instance.UpdateLandByPos(pos, land, tx);
        }

        public async Task<Dictionary<int, LoadLandInfo>> GetLandCell(int[] pos)
        {
            var cells = await LandDicDataHelper.Instance.GetLandDicByPos(pos);
            if (cells != null && cells.Any())
            {
                Dictionary<int, LoadLandInfo> list = new Dictionary<int, LoadLandInfo>();
                foreach (var i in cells)
                {
                    list.Add(i.Key, new LoadLandInfo()
                    {
                        PosX = i.Value.PosX,
                        PoxY = i.Value.PoxY,
                        State = i.Value.State,
                        RoleId = i.Value.RoleId
                    });
                }
                return list;
            }
            return null;
        }


        public async Task<BaseResponseData> BuyLand(int pos, BuyLandResult result)
        {
            if (pos > MaxPosValue)
            {
                result.Result = GameEnum.WsResult.ParamsError;
                return result;
            }
            //获取土地状态
            var land = await LandDataHelper.Instance.GetLandByPos(pos);

            if (land == null || land.State == (int)MapStatus.Empty)
            {//可以购买
                result = await BuyLand(pos);
                return result;
            }
            else if (land.State == (int)MapStatus.Saled)
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

        public async Task<BuyLandResult> BuyLand(int pos)
        {
            BuyLandResult result = new BuyLandResult();
            var config = AutoData.LandInfo.GetForLevel(1);
            var role = LogicServer.User.role;

            LandData land = new LandData(pos, role.Id);

            var posList = await PosListDataHelper.Instance.GetPosListByRoleIdAsync(role.Id);
            if (posList == null)
            {
                posList = new List<int>();
            }
            posList.Add(pos);
            Dictionary<int, LandData> dic = await LandDicDataHelper.Instance.GetLandDicByRoleIdAsync(role.Id);
            if (dic == null)
            {
                dic = new Dictionary<int, LandData>();
            }
            dic.Add(pos, land);
            FinanceLogData log = new FinanceLogData()
            {
                AorD = false,
                Count = -config.Price.Count,
                MoneyType = config.Price.CurrencyID,
                Type = (int)GameEnum.FinanceLog.BuyLand
            };
            try
            {
                using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
                {
                    await LandDicDataHelper.Instance.UpdateLandDicByRoleIdAsync(role.Id, dic, tx);
                    await BagDataHelper.Instance.DecGold(config.Price.Count, config.Price.CurrencyID, tx);
                    await LandDataHelper.Instance.UpdateLandByPos(pos, land, tx);
                    await PosListDataHelper.Instance.UpdatePosListByRoleIdAsync(role.Id, posList, tx);
                    await RoleController.Instance.AddIncome(role.Id, config.Income, tx);
                    await FinanceLogController.Instance.UpdateFinanceLog(role.Id, log, tx);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                result.Result = GameEnum.WsResult.BuyLandErr;
                return result;
            }
            result.PosX = land.PosX;
            result.PoxY = land.PoxY;
            result.State = land.State;
            result.RoleId = land.RoleId;

            await MsgSender.Instance.UpdateIncome();  //更新身价
            await MsgSender.Instance.GoldUpdate(config.Price.CurrencyID);
            await MsgSender.Instance.FinanceLogUpdate(log);
            return result;



        }


    }
}
