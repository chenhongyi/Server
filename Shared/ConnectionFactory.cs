using LogicServer.Interface;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class ConnectionFactory
    {
        public static readonly string WebSocketServerName = @"ws://localhost:5555/WebSocket/";
        public static readonly string PublicGateApi = "http://localhost:5555/Account/";
        public static readonly string ReserveStockApiController = PublicGateApi + "reservestock";
        public static readonly string StockApiController = PublicGateApi + "stockaggregator";
        public static readonly string StockAggregatorApiController = PublicGateApi + "stock";
        public static readonly Uri StockAggregatorServiceUri = new Uri("fabric:/Server/StockAggregatorService");
        public static readonly Uri StockServiceUri = new Uri("fabric:/Server/StockService");
        public static readonly Uri LogicServiceUri = new Uri("fabric:/Server/LogicServer");
        private static readonly string ApplicationName = "fabric:/Server";



        public static ILogicServer CreateAccountService()
        {
            return ServiceProxy.Create<ILogicServer>(LogicServiceUri, new ServicePartitionKey(0));
        }

    

        //public static IStockService CreateStockService(int productId)
        //{
        //    return ServiceProxy.Create<IStockService>(StockServiceUri, new ServicePartitionKey(productId));
        //}

        //public static IStockAggregatorService CreateStockAggregatorService(int productId)
        //{
        //    return ServiceProxy.Create<IStockAggregatorService>(StockAggregatorServiceUri, new ServicePartitionKey(productId));
        //}

        //public static IStockTrendPredictionActor CreateStockTrendPredictionActor(int productId)
        //{
        //    return ActorProxy.Create<IStockTrendPredictionActor>(new ActorId(productId), ApplicationName);
        //}


        //TODO： actor
        //public static INotificationActor CreateNotificationActor()
        //{
        //    return ActorProxy.Create<INotificationActor>(new ActorId(0), ApplicationName);
        //}

        //public static IStockTrendPredictionActor CreateBatchedStockTrendPredictionActor(string actorId)
        //{
        //    return ActorProxy.Create<IStockTrendPredictionActor>(new ActorId(actorId), ApplicationName);
        //}
    }
}
