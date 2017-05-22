using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Model;
using PublicGate.Comms;
using Shared;
using Shared.Serializers;
using Shared.Websockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PublicGate
{
    class PublicGateWebSocketConnectionHandler : IWebSocketConnectionHandler
    {
        private readonly ICommunicationClientFactory<WsCommunicationClient> clientFactory
            = new WsCommunicationClientFactory();
        public async Task<byte[]> ProcessWsMessageAsync(
           byte[] wsrequest, string sessionId, CancellationToken cancellationToken
           )
        {
            IWsSerializer mserializer = new ProtobufWsSerializer();
            WsRequestMessage mrequest = await mserializer.DeserializeAsync<WsRequestMessage>(wsrequest);

            ServicePartitionClient<WsCommunicationClient> serviceClient =
                new ServicePartitionClient<WsCommunicationClient>(
                    this.clientFactory,
                    //ConnectionFactory.StockServiceUri,
                    ConnectionFactory.LogicServiceUri,
                    // ConnectionFactory.MapServiceUri,
                    partitionKey: new ServicePartitionKey(mrequest.PartitionKey),    //选择服务器集群中的一个partition
                    listenerName: ServiceConst.ListenerWebsocket);

            return await serviceClient.InvokeWithRetryAsync(async client => await client.SendReceiveAsync(wsrequest),  cancellationToken);
        }

        public async Task ProcessWsMessageAsync(string sessionId, CancellationToken cancellationToken)
        {
            ServicePartitionClient<WsCommunicationClient> serviceClient =
                new ServicePartitionClient<WsCommunicationClient>(
                    this.clientFactory,
                    //ConnectionFactory.StockServiceUri,
                    ConnectionFactory.LogicServiceUri,
                    // ConnectionFactory.MapServiceUri,
                    partitionKey: new ServicePartitionKey(0),    //选择服务器集群中的一个partition
                    listenerName: ServiceConst.ListenerWebsocket);

            await serviceClient.InvokeWithRetryAsync(async client => await client.SendReceiveAsync(sessionId), cancellationToken);
        }
    }
}
