﻿using System;
using System.Fabric;
using System.Fabric.Description;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Logging;
using Microsoft.Owin.Hosting;

namespace PublicGate
{


    //只跟http的服务有关
    internal class OwinCommunicationListener : ICommunicationListener
    {
        private static readonly ILogger Logger = LoggerFactory.GetLogger(nameof(OwinCommunicationListener));
        private readonly string appRoot;
        private readonly IOwinAppBuilder startup;
        private readonly StatelessServiceContext serviceContext;
        private string listeningAddress;
        private IDisposable serverHandle;

        public OwinCommunicationListener(string appRoot, IOwinAppBuilder startup, StatelessServiceContext serviceContext)
        {
            this.appRoot = appRoot;
            this.startup = startup;
            this.serviceContext = serviceContext;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            Logger.Debug(nameof(this.OpenAsync));

            try
            {
                EndpointResourceDescription serviceEndpoint = this.serviceContext.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
                int port = serviceEndpoint.Port;

                this.listeningAddress = String.Format(
                    CultureInfo.InvariantCulture,
                    "http://+:{0}/{1}",
                    port,
                    string.IsNullOrWhiteSpace(this.appRoot)
                        ? string.Empty
                        : this.appRoot.TrimEnd('/') + '/');

                this.serverHandle = WebApp.Start(this.listeningAddress, appBuilder => this.startup.Configuration(appBuilder));

                string resultAddress = this.listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

                Logger.Debug("Listening on {0}", resultAddress);

                return Task.FromResult(resultAddress);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, nameof(this.OpenAsync));
                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            Logger.Debug(nameof(this.CloseAsync));

            this.StopWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            Logger.Debug(nameof(this.Abort));

            this.StopWebServer();
        }

        private void StopWebServer()
        {
            if (this.serverHandle != null)
            {
                try
                {
                    this.serverHandle.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // no-op
                }
            }
        }
    }
}
