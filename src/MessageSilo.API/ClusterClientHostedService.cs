using MessageSilo.Features.Connection;
using MessageSilo.Features.EntityManager;
using MessageSilo.Features.UserManager;
using MessageSilo.Shared.Enums;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Polly;
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

            var siloIP = IPAddress.Parse(configuration["PrimarySiloAddress"]);

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

            var retry = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            await retry.ExecuteAsync(async () => await Client.Connect());

            //Init connections
            var um = Client.GetGrain<IUserManagerGrain>("um");
            var users = await um.GetAll();

            foreach (var user in users)
            {
                var em = Client.GetGrain<IEntityManagerGrain>(user);
                var connections = (await em.GetAll()).Where(p => p.Kind == EntityKind.Connection);

                foreach (var entity in connections)
                {
                    var conn = Client.GetGrain<IConnectionGrain>(entity.Id);
                    await conn.GetState();
                    logger.LogInformation($"Connection ({entity.Id}) initialized.");
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Client.Close();

            Client.Dispose();
        }
    }
}
