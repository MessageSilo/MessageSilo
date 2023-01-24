using MessageSilo.API.Controllers;
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

            Client = new ClientBuilder()
                        .UseStaticClustering(new IPEndPoint(IPAddress.Parse("10.5.0.5"), 30000))
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "MessageSiloCluster001";
                            options.ServiceId = "MessageSiloService001";
                        })
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
