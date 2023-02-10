﻿using MessageSilo.API.Controllers;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;

namespace MessageSilo.API
{
    public class ClusterClientHostedService : IHostedService
    {
        public IClusterClient Client { get; }

        protected readonly ILogger<ClusterClientHostedService> logger;

        public ClusterClientHostedService(ILoggerProvider loggerProvider, IConfiguration configuration, ILogger<ClusterClientHostedService> logger)
        {
            this.logger = logger;

            IClientBuilder clientBuilder = new ClientBuilder();

            var siloIP = IPAddress.Parse(configuration["Orleans:PrimarySiloAddress"]);

            if (siloIP.Equals(IPAddress.Loopback))
                clientBuilder = clientBuilder.UseLocalhostClustering();
            else
                clientBuilder = clientBuilder
                    .UseStaticClustering(new IPEndPoint(siloIP, 30000))
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "MessageSiloCluster001";
                        options.ServiceId = "MessageSiloService001";
                    });

            Client = clientBuilder
                        .ConfigureLogging(builder => builder.AddProvider(loggerProvider))
                        .Build();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // A retry filter could be provided here.
            await Client.Connect();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Client.Close();

            Client.Dispose();
        }
    }
}