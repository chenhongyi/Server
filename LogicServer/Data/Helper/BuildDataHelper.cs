using Microsoft.ServiceFabric.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class BuildDataHelper : DataHelperBase<BuildDataHelper, Model.Data.Business.BuildData>
    {


        public async Task<Model.Data.Business.BuildData> GetBuildByBuildId(string id)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var build = await db.TryGetValueAsync(tx, id);
                return build.HasValue ? build.Value : null;
            }
        }

        public async Task SetBuildByBuildId(Model.Data.Business.BuildData data)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, data.Id, data);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateBuildByBuildId(Model.Data.Business.BuildData data)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, data.Id, data);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveBuildByBuildId(string id)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, id);
                await tx.CommitAsync();
            }
        }

        public async Task SetBuildByBuildId(Model.Data.Business.BuildData data, ITransaction tx)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            await db.AddAsync(tx, data.Id, data);
        }

        public async Task UpdateBuildByBuildId(Model.Data.Business.BuildData data, ITransaction tx)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            await db.SetAsync(tx, data.Id, data);
        }

        public async Task RemoveBuildByBuildId(string id, ITransaction tx)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            await db.TryRemoveAsync(tx, id);
        }
    }



    public class BuildIdDataHelper : DataHelperBase<BuildIdDataHelper, List<string>>
    {

        #region roleid


        public async Task<List<string>> GetBuildIdListByRoleId(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var id = await db.TryGetValueAsync(tx, roleId);
                return id.HasValue ? id.Value : null;
            }
        }

        public async Task SetBuildIdListByRoleId(Guid roleId, List<string> buildId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleId, buildId);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateBuildIdListByRoleId(Guid roleId, List<string> buildId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, roleId, buildId);
                await tx.CommitAsync();
            }
        }


        public async Task SetBuildIdListByRoleId(Guid roleId, string buildId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            List<string> list = new List<string>();
            list.Add(buildId);
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, roleId, list);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateBuildIdByRoleId(Guid roleId, string buildId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            var list = await GetBuildIdListByRoleId(roleId);
            if (list != null)
            {
                list.Add(buildId);
            }
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, roleId, list);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveBuildIdByRoleId(Guid roleId)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, roleId);
                await tx.CommitAsync();
            }
        }

        public async Task SetBuildIdListByRoleId(Guid roleId, List<string> buildId, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.AddAsync(tx, roleId, buildId);
        }

        public async Task UpdateBuildIdListByRoleId(Guid roleId, List<string> buildId, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.SetAsync(tx, roleId, buildId);
        }


        public async Task SetBuildIdListByRoleId(Guid roleId, string buildId, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            List<string> list = new List<string>();
            list.Add(buildId);
            await db.AddAsync(tx, roleId, list);
        }

        public async Task UpdateBuildIdListByRoleId(Guid roleId, string buildId, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            var list = await GetBuildIdListByRoleId(roleId);
            if (list != null)
            {
                list.Add(buildId);
            }
            await db.SetAsync(tx, roleId, list);
        }

        public async Task RemoveBuildIdByRoleId(Guid roleId, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.TryRemoveAsync(tx, roleId);
        }


        #endregion
    }
    public class BuildIdPosDataHelper : DataHelperBase<BuildIdPosDataHelper, string>
    {
        #region pos

        public async Task<string> GetBuildIdByPos(int pos)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var id = await db.TryGetValueAsync(tx, pos);
                return id.HasValue ? id.Value : null;
            }
        }


        public async Task SetBuildIdByPos(int pos, string buildId)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, pos, buildId);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateBuildIdByPos(int pos, string buildId)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, pos, buildId);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveBuildIdByPos(int pos)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, pos);
                await tx.CommitAsync();
            }
        }

        public async Task SetBuildIdByPos(int pos, string buildId, ITransaction tx)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            await db.AddAsync(tx, pos, buildId);
        }

        public async Task UpdateBuildIdByPos(int pos, string buildId, ITransaction tx)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            await db.SetAsync(tx, pos, buildId);
        }

        public async Task RemoveBuildIdByPos(int pos, ITransaction tx)
        {
            var db = GetInt(LogicServer.Instance.StateManager).Result;
            await db.TryRemoveAsync(tx, pos);
        }
        #endregion
    }
}
