using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Shared.Websockets;
using Shared;
using Logging;
using Model;
using Shared.Serializers;

namespace MapServer
{
    /// <summary>
    /// 通过 Service Fabric 运行时为每个服务副本创建此类的一个实例。
    /// </summary>
    internal sealed class MapServer : StatefulService, IWebSocketConnectionHandler
    {
        private const string AccountsCollectionName = "accounts";
        private const string TokenCollectionName = "tokens";
        private const string PassportCollectionName = "passports";
        private const string LoginCollectionName = "logins";
        private IReliableDictionary<string, Account> accountsCollection = null;   //账号数据
        private IReliableDictionary<Guid, Token> tokenCollection = null;      //token数据
        private IReliableDictionary<string, Passport> passportCollection = null;      //通行证数据
        private IReliableDictionary<string, Login> loginCollection = null;      //Login数据
        private static readonly ILogger Logger = LoggerFactory.GetLogger(nameof(MapServer));
        public MapServer(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<byte[]> ProcessWsMessageAsync(byte[] wsrequest, CancellationToken cancellationToken)
        {
            Logger.Debug(nameof(this.ProcessWsMessageAsync));

            this.accountsCollection = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Account>>(AccountsCollectionName);

            using (var tx = this.StateManager.CreateTransaction())
            {
                var s= await this.accountsCollection.TryGetValueAsync(tx, "123");
                if (s.HasValue)
                {
                    Logger.Debug(s.Value.AccountID.ToString());
                }
            }
            ProtobufWsSerializer mserializer = new ProtobufWsSerializer();

            WsRequestMessage mrequest = await mserializer.DeserializeAsync<WsRequestMessage>(wsrequest);

            ///事件集中在这里处理  并且可以保存状态
            switch (mrequest.MsgId)
            {
                case WSMsgID.CreateRole:
                    {
                        IWsSerializer pserializer = SerializerFactory.CreateSerializer();
                        //创建角色数据 PostProductModel payload = await pserializer.DeserializeAsync<PostProductModel>(mrequest.Value);    //取值
                        //执行 await this.PurchaseProduct(payload.ProductId, payload.Quantity);    //数据处理
                    }
                    break;
            }

            //构造返回值
            //PostProductModel m = new PostProductModel();
            //m.ProductId = 100;
            //m.Quantity = 200;
            //var m1 = await mserializer.SerializeAsync(m);

            //构造返回值
            WsResponseMessage mresponse = new WsResponseMessage
            {
                Result = WsResult.Success,
                Value = null
            };

            return await mserializer.SerializeAsync(mresponse);
        }


        /// <summary>
        ///可选择性地替代以创建侦听器(如 HTTP、服务远程、WCF 等)，从而使此服务副本可处理客户端或用户请求。
        /// </summary>
        /// <remarks>
        ///有关服务通信的详细信息，请参阅 https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>侦听器集合。</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new List<ServiceReplicaListener>()
            { new ServiceReplicaListener(
                  context =>
                  new WebSocketListener("WsServiceEndpoint", "MapServerWS", context, () => this),
              ServiceConst.ListenerWebsocket)
            };
        }

        /// <summary>
        /// 这是服务副本的主入口点。
        /// 在此服务副本成为主服务并具有写状态时，将执行此方法。
        /// </summary>
        /// <param name="cancellationToken">已在 Service Fabric 需要关闭此服务副本时取消。</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: 将以下示例代码替换为你自己的逻辑 
            //       或者在服务不需要此 RunAsync 重写时删除它。

           // var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            //while (true)
            //{
            //    cancellationToken.ThrowIfCancellationRequested();

            //    using (var tx = this.StateManager.CreateTransaction())
            //    {
            //        var result = await myDictionary.TryGetValueAsync(tx, "Counter");

            //        ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
            //            result.HasValue ? result.Value.ToString() : "Value does not exist.");

            //        await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

            //        // 如果在调用 CommitAsync 前引发异常，则将终止事务，放弃 
            //        // 所有更改，并且辅助副本中不保存任何内容。
            //        await tx.CommitAsync();
            //    }

            //    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            //}
        }
    }
}
