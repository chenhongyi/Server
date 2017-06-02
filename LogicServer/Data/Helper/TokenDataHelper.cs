using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicServer.Data.Helper
{
    public class TokenDataHelper : DataHelperBase<TokenDataHelper, Model.Data.Account.Token>
    {
        public async Task<Model.Data.Account.Token> GetTokenBySignToken(string signToken)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                var token = await db.TryRemoveAsync(tx, signToken);
                return token.HasValue ? token.Value : null;
            }
        }

        public async Task SetTokenBySignToken(string signToken, Model.Data.Account.Token token)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.AddAsync(tx, signToken, token);
                await tx.CommitAsync();
            }
        }

        public async Task UpdateTokenBySignToken(string signToken, Model.Data.Account.Token token)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.SetAsync(tx, signToken, token);
                await tx.CommitAsync();
            }
        }

        public async Task RemoveTokenBySignToken(string signToken)
        {
            var db = GetString(LogicServer.Instance.StateManager).Result;
            using (var tx = LogicServer.Instance.StateManager.CreateTransaction())
            {
                await db.TryRemoveAsync(tx, signToken);
                await tx.CommitAsync();
            }
        }
    }
}
