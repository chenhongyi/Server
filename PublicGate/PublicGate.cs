using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Logging;
using Shared;
using Shared.Websockets;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace PublicGate
{
    /// <summary>
    /// 通过 Service Fabric 运行时为每个服务实例创建此类的一个实例。
    /// </summary>
    public sealed class PublicGate : StatelessService
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger(nameof(PublicGate));
        public PublicGate(StatelessServiceContext context)
            : base(context)
        { }

        public Task SendMsgToAllClients()
        {
            throw new NotImplementedException();
        }

        public Task SendMsgToClients()
        {
            throw new NotImplementedException();
        }

        public Task SendMsgToSignleClient()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 可选择性地替代以创建侦听器(如 TCP、HTTP)，从而使此服务副本可以处理客户端或用户请求。
        /// </summary>
        /// <returns>侦听器集合。</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[] {
                new ServiceInstanceListener(        //websocket
                    initParams => new WebSocketListener(null, "WebSocket", initParams, () => new PublicGateWebSocketConnectionHandler()),
                    ServiceConst.ListenerWebsocket)
            };
        }

        /// <summary>
        /// 这是服务实例的主入口点。
        /// </summary>
        /// <param name="cancellationToken">已在 Service Fabric 需要关闭此服务实例时取消。</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: 将以下示例代码替换为你自己的逻辑 
            //       或者在服务不需要此 RunAsync 重写时删除它。
            Logger.Debug(nameof(this.RunAsync));

            //TODO: actor
            //try
            //{
            //    INotificationActor notificationActor = ConnectionFactory.CreateNotificationActor();
            //    await notificationActor.SubscribeAsync(new StockEventsHandler());

            //    await base.RunAsync(cancellationToken);
            //}
            //catch (Exception ex)
            //{
            //    Logger.Error(ex, nameof(this.RunAsync));
            //    throw;
            //}
        }
    }
}
