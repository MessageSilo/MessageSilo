using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;

namespace MessageSilo.API
{
    public class ClusterClientHostedService : IHostedService
    {
        public IClusterClient Client { get; }

        public ClusterClientHostedService(ILoggerProvider loggerProvider, IConfiguration configuration)
        {
            Client = new ClientBuilder()
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "MessageSiloCluster001";
                            options.ServiceId = "MessageSiloService001";
                        })
                        .UseStaticClustering(IPEndPoint.Parse("127.0.0.1:30000"))
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
