using Microsoft.ServiceFabric.Data;
using Model.Data.Npc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class BagDataHelper : DataHelperBase<BagDataHelper, Model.Data.Npc.Bag>
    {

        public async Task<Model.Data.Npc.Bag> GetBagByRoleId(Guid id)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var bag = await db.TryGetValueAsync(tx, id);
                return bag.HasValue ? bag.Value : null;
            }
        }

        public async Task SetBagByRoleId(Bag bag, Guid id)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, id, bag);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateBagByRoleId(Bag bag, Guid id)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, id, bag);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveBagByRoleId(Guid id)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, id);
                await tx.CommitAsync();
            }
        }
        public async Task UpdateBagByRoleId(Bag bag, Guid id, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.SetAsync(tx, id, bag);
        }
        public async Task SetBagByRoleId(Bag bag, Guid id, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.AddAsync(tx, id, bag);
        }
        public async Task RemoveBagByRoleId(Guid id, ITransaction tx)
        {
            var db = GetGuid(LogicServer.Instance.StateManager).Result;
            await db.TryRemoveAsync(tx, id);
        }

        public async Task DecGold(long count, int type, ITransaction tx)
        {
            LogicServer.User.bag.Items.TryGetValue(type, out Model.Data.General.Item money);
            if (money.CurCount >= count)
            {
                money.CurCount -= count;
            }
            await UpdateBagByRoleId(LogicServer.User.bag, LogicServer.User.role.Id, tx);

        }
        public async Task DecGold(long count, int type)
        {
            LogicServer.User.bag.Items.TryGetValue(type, out Model.Data.General.Item money);
            if (money.CurCount >= count)
            {
                money.CurCount -= count;
            }
            await UpdateBagByRoleId(LogicServer.User.bag, LogicServer.User.role.Id);
            // await MsgSender.Instance.GoldUpdate(count, type);
        }


        public async Task AddGold(long count, int type, ITransaction tx)
        {
            LogicServer.User.bag.Items.TryGetValue(type, out Model.Data.General.Item money);
            if (count >= 0)
            {
                money.CurCount += count;
            }
            await UpdateBagByRoleId(LogicServer.User.bag, LogicServer.User.role.Id, tx);

        }
        public async Task AddGold(long count, int type)
        {
            LogicServer.User.bag.Items.TryGetValue(type, out Model.Data.General.Item money);
            if (count >= 0)
            {
                money.CurCount += count;
            }
            await UpdateBagByRoleId(LogicServer.User.bag, LogicServer.User.role.Id);
            // await MsgSender.Instance.GoldUpdate(count, type);
        }
    }
}
