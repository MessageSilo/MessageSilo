using MessageSilo.Features.Connection;
using MessageSilo.Features.MessageCorrector;
using MessageSilo.Shared.DataAccess;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile("appsettings.Development.json", true, true)
    .Build();

var siloIP = IPAddress.Parse(configuration["Orleans:PrimarySiloAddress"]);

var builder = Host.CreateDefaultBuilder(args)
        .UseOrleans(siloBuilder =>
        {
            if (siloIP.Equals(IPAddress.Loopback))
                siloBuilder = siloBuilder.UseLocalhostClustering();
            else
                siloBuilder = siloBuilder.UseDevelopmentClustering(primarySiloEndpoint: new IPEndPoint(siloIP, 11111))
                            .Configure<ClusterOptions>(options =>
                            {
                                options.ClusterId = "MessageSiloCluster001";
                                options.ServiceId = "MessageSiloService001";
                            })
                            .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000);

            siloBuilder = siloBuilder.ConfigureApplicationParts(manager =>
            {
                manager.AddApplicationPart(typeof(IConnectionGrain).Assembly);
                manager.AddApplicationPart(typeof(IMessageCorrectorGrain).Assembly);
            })
            .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Debug).AddConsole())
            .AddAzureTableGrainStorageAsDefault(options =>
            {
                options.ConfigureTableServiceClient(configuration["ConnectionStrings:GrainStateStorage"]);
                options.UseJson = true;
            });
        });

builder.ConfigureServices(services =>
{
    services.AddSingleton<IMessageRepository<CorrectedMessage>, MessageRepository<CorrectedMessage>>();
});

var app = builder.Build();

app.Run();

if (siloIP.Equals(IPAddress.Loopback))
    Console.ReadKey();