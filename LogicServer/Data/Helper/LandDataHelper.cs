using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class LandDataHelper : DataHelperBase<LandDataHelper, Model.Data.Business.LandData>
    {
        #region Guid   LandProperty
        public async Task<Model.Data.Business.LandData> GetLandByPos(int pos)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var land = await db.TryGetValueAsync(tx, pos);
                return land.HasValue ? land.Value : null;
            }
        }

        public async Task SetLandByPos(int pos, Model.Data.Business.LandData land)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, pos, land);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateLandByPos(int pos, Model.Data.Business.LandData land)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, pos, land);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveLandByPos(int pos)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, pos);
                await tx.CommitAsync();
            }
        }


        public async Task SetLandByPos(int pos, Model.Data.Business.LandData land, ITransaction tx)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            await db.AddAsync(tx, pos, land);
        }

        public async Task UpdateLandByPos(int pos, Model.Data.Business.LandData land, ITransaction tx)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            await db.SetAsync(tx, pos, land);
        }

        public async Task RemoveLandByPos(int pos, ITransaction tx)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            await db.TryRemoveAsync(tx, pos);
        }

        #endregion

    }

    public class LandDicDataHelper : DataHelperBase<LandDicDataHelper, Dictionary<int, Model.Data.Business.LandData>>
    {

        public async Task<Dictionary<int, Model.Data.Business.LandData>> GetLandDicByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var mapDic = await db.TryGetValueAsync(tx, roleId);
                return mapDic.HasValue ? mapDic.Value : null;
            }
        }

        public async Task SetLandDicByRoleIdAsync(Guid roleId, Dictionary<int, Model.Data.Business.LandData> landDic)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleId, landDic);
                await tx.CommitAsync();
            }
        }


        public async Task UpdateLandDicByRoleIdAsync(Guid roleId, Dictionary<int, Model.Data.Business.LandData> landDic)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, roleId, landDic);
                await tx.CommitAsync();
            }
        }


        public async Task RemoveLandDicByRoleIdAsync(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, roleId);
                await tx.CommitAsync();
            }
        }


        public async Task SetLandDicByRoleIdAsync(Guid roleId, Dictionary<int, Model.Data.Business.LandData> landDic, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
                await db.AddAsync(tx, roleId, landDic);
        }


        public async Task UpdateLandDicByRoleIdAsync(Guid roleId, Dictionary<int, Model.Data.Business.LandData> landDic, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
                await db.SetAsync(tx, roleId, landDic);
        }


        public async Task RemoveLandDicByRoleIdAsync(Guid roleId, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
                await db.TryRemoveAsync(tx, roleId);
        }

        public async Task<Dictionary<int, Model.Data.Business.LandData>> GetLandDicByPos(int[] pos)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            Dictionary<int, Model.Data.Business.LandData> dic = new Dictionary<int, Model.Data.Business.LandData>();

            foreach (var p in pos)
            {
                var land = await LandDataHelper.Instance.GetLandByPos(p);
                if (land != null)
                {
                    dic.Add(p, land);
                }
            }
            return dic;
        }
    }
}
