using LogicServer.Interface;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using PublicGate.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class ConnectionFactory
    {


        public static readonly Uri LogicServiceUri = new Uri("fabric:/Server/LogicServer");
        public static readonly Uri PublicGateUri = new Uri("fabric:/Server/PublicGate");



        public static ILogicServer CreateAccountService()
        {
            return ServiceProxy.Create<ILogicServer>(LogicServiceUri, new ServicePartitionKey(0));
        }


        public static ILogicServer CreateLogicService()
        {
            return ServiceProxy.Create<ILogicServer>(LogicServiceUri, new ServicePartitionKey(0));
        }

        public static IPublicGate CreatePublicGateService()
        {
            return ServiceProxy.Create<IPublicGate>(PublicGateUri);
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
