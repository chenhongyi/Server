using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class DataHelperBase<TDB, TData>
    {
        // [ThreadStatic]
        private static IReliableStateManager sm = LogicServer.Instance.StateManager;
        private static TDB _Instance;
        public static TDB Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = System.Activator.CreateInstance<TDB>();
                }
                return _Instance;
            }
        }

        protected DataHelperBase()
        {
            SelfName = typeof(TDB).Name;
        }

        private string SelfName { get; set; }
        protected async Task<IReliableDictionary<Guid, TData>> GetGuid(IReliableStateManager sm)
        {
            var db = await sm.GetOrAddAsync<IReliableDictionary<Guid, TData>>(SelfName + "Guid");
            return db;
        }

        protected async Task<IReliableDictionary<string, TData>> GetString(IReliableStateManager sm)
        {
            var db = await sm.GetOrAddAsync<IReliableDictionary<string, TData>>(SelfName + "String");
            return db;
        }

        protected async Task<IReliableDictionary<int, TData>> GetInt(IReliableStateManager sm)
        {
            var db = await sm.GetOrAddAsync<IReliableDictionary<int, TData>>(SelfName + "Int");
            return db;
        }

        protected async Task<IReliableDictionary<long, TData>> GetLong(IReliableStateManager sm)
        {
            var db = await sm.GetOrAddAsync<IReliableDictionary<long, TData>>(SelfName + "Long");
            return db;
        }

        #region Key = string类型的

        public async Task<TData> Get(string key)
        {
            var db = await GetString(sm);
            using (var tx = sm.CreateTransaction())
            {
                var value = await db.TryGetValueAsync(tx, key);
                return value.HasValue ? value.Value : (TData)(object)null;
            }
        }

        public async Task Set(string key, TData data)
        {
            var db = await GetString(sm);
            using (var tx = sm.CreateTransaction())
            {
                await db.AddAsync(tx, key, data);
                await tx.CommitAsync();
            }
        }

        public async Task Update(string key, TData data)
        {
            var db = await GetString(sm);
            using (var tx = sm.CreateTransaction())
            {
                await db.SetAsync(tx, key, data);
                await tx.CommitAsync();
            }
        }

        public async Task Remove(string key)
        {
            var db = await GetString(sm);
            using (var tx = sm.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, key);
                await tx.CommitAsync();
            }
        }


        public async Task Set(string key, TData data, ITransaction tx)
        {
            var db = await GetString(sm);
            await db.AddAsync(tx, key, data);
        }

        public async Task Update(string key, TData data, ITransaction tx)
        {
            var db = await GetString(sm);
            await db.SetAsync(tx, key, data);
        }

        public async Task Remove(string key, ITransaction tx)
        {
            var db = await GetString(sm);
            await db.TryRemoveAsync(tx, key);
        }
        #endregion



        #region Key = int类型的

        public async Task<TData> Get(int key)
        {
            var db = await GetInt(sm);
            using (var tx = sm.CreateTransaction())
            {
                var value = await db.TryGetValueAsync(tx, key);
                return value.HasValue ? value.Value : (TData)(object)null;
            }
        }

        public async Task Set(int key, TData data)
        {
            var db = await GetInt(sm);
            using (var tx = sm.CreateTransaction())
            {
                await db.AddAsync(tx, key, data);
                await tx.CommitAsync();
            }
        }

        public async Task Update(int key, TData data)
        {
            var db = await GetInt(sm);
            using (var tx = sm.CreateTransaction())
            {
                await db.SetAsync(tx, key, data);
                await tx.CommitAsync();
            }
        }

        public async Task Remove(int key)
        {
            var db = await GetInt(sm);
            using (var tx = sm.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, key);
                await tx.CommitAsync();
            }
        }


        public async Task Set(int key, TData data, ITransaction tx)
        {
            var db = await GetInt(sm);
            await db.AddAsync(tx, key, data);
        }

        public async Task Update(int key, TData data, ITransaction tx)
        {
            var db = await GetInt(sm);
            await db.SetAsync(tx, key, data);
        }

        public async Task Remove(int key, ITransaction tx)
        {
            var db = await GetInt(sm);
            await db.TryRemoveAsync(tx, key);
        }
        #endregion

        #region Key = long类型的

        public async Task<TData> Get(long key)
        {
            var db = await GetLong(sm);
            using (var tx = sm.CreateTransaction())
            {
                var value = await db.TryGetValueAsync(tx, key);
                return value.HasValue ? value.Value : (TData)(object)null;
            }
        }

        public async Task Set(long key, TData data)
        {
            var db = await GetLong(sm);
            using (var tx = sm.CreateTransaction())
            {
                await db.AddAsync(tx, key, data);
                await tx.CommitAsync();
            }
        }

        public async Task Update(long key, TData data)
        {
            var db = await GetLong(sm);
            using (var tx = sm.CreateTransaction())
            {
                await db.SetAsync(tx, key, data);
                await tx.CommitAsync();
            }
        }

        public async Task Remove(long key)
        {
            var db = await GetLong(sm);
            using (var tx = sm.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, key);
                await tx.CommitAsync();
            }
        }


        public async Task Set(long key, TData data, ITransaction tx)
        {
            var db = await GetLong(sm);
            await db.AddAsync(tx, key, data);
        }

        public async Task Update(long key, TData data, ITransaction tx)
        {
            var db = await GetLong(sm);
            await db.SetAsync(tx, key, data);
        }

        public async Task Remove(long key, ITransaction tx)
        {
            var db = await GetLong(sm);
            await db.TryRemoveAsync(tx, key);
        }
        #endregion


        #region Key = long类型的

        public async Task<TData> Get(Guid key)
        {
            var db = await GetGuid(sm);
            using (var tx = sm.CreateTransaction())
            {
                var value = await db.TryGetValueAsync(tx, key);
                return value.HasValue ? value.Value : (TData)(object)null;
            }
        }

        public async Task Set(Guid key, TData data)
        {
            var db = await GetGuid(sm);
            using (var tx = sm.CreateTransaction())
            {
                await db.AddAsync(tx, key, data);
                await tx.CommitAsync();
            }
        }

        public async Task Update(Guid key, TData data)
        {
            var db = await GetGuid(sm);
            using (var tx = sm.CreateTransaction())
            {
                await db.SetAsync(tx, key, data);
                await tx.CommitAsync();
            }
        }

        public async Task Remove(Guid key)
        {
            var db = await GetGuid(sm);
            using (var tx = sm.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, key);
                await tx.CommitAsync();
            }
        }


        public async Task Set(Guid key, TData data, ITransaction tx)
        {
            var db = await GetGuid(sm);
            await db.AddAsync(tx, key, data);
        }

        public async Task Update(Guid key, TData data, ITransaction tx)
        {
            var db = await GetGuid(sm);
            await db.SetAsync(tx, key, data);
        }

        public async Task Remove(Guid key, ITransaction tx)
        {
            var db = await GetGuid(sm);
            await db.TryRemoveAsync(tx, key);
        }
        #endregion
    }
}