using MessageSilo.Features.DeadLetterCorrector;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Grains;
using Orleans;
using Orleans.Hosting;
using MessageSilo.Shared.DataAccess;
using Orleans.Configuration;
using System.Net;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile("appsettings.Development.json", true, true)
    .Build();

var builder = Host.CreateDefaultBuilder(args)
        .UseOrleans(siloBuilder => siloBuilder
        .ConfigureApplicationParts(manager =>
        {
            manager.AddApplicationPart(typeof(IUserGrain).Assembly);
            manager.AddApplicationPart(typeof(IDeadLetterCorrectorGrain).Assembly);
        })
        .UseDevelopmentClustering(primarySiloEndpoint: new IPEndPoint(IPAddress.Parse("10.5.0.5"), 11111))
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "MessageSiloCluster001";
                    options.ServiceId = "MessageSiloService001";
                })
        .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
        .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Warning).AddConsole())
        .AddMemoryGrainStorageAsDefault());

builder.ConfigureServices(services =>
{
    services.AddScoped<IMessageCorrector, MessageCorrector>();
    services.AddSingleton<IMessageRepository<CorrectedMessage>, MessageRepository<CorrectedMessage>>();
});

var app = builder.Build();

app.Run();
