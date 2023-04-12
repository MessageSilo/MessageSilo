using MessageSilo.Features.Connection;
using MessageSilo.Features.MessageCorrector;
using MessageSilo.Features.Target;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Models;
using Microsoft.ApplicationInsights.Extensibility;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Serilog;
using System.Net;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile("appsettings.Development.json", true, true)
    .Build();

var siloIP = IPAddress.Parse(configuration["Orleans:PrimarySiloAddress"]);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateBootstrapLogger();

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
                manager.AddApplicationPart(typeof(IMessageSenderGrain).Assembly);
                manager.AddApplicationPart(typeof(ITargetGrain).Assembly);
                manager.AddApplicationPart(typeof(IConnectionGrain).Assembly);
                manager.AddApplicationPart(typeof(IMessageCorrectorGrain).Assembly);
            })
            .AddAzureTableGrainStorageAsDefault(options =>
            {
                options.ConfigureTableServiceClient(configuration["ConnectionStrings:GrainStateStorage"]);
                options.UseJson = true;
                options.DeleteStateOnClear = true;
            });
        });

builder.ConfigureServices(services =>
{
    services.AddSingleton<IMessageRepository<CorrectedMessage>, MessageRepository<CorrectedMessage>>();
});

builder.UseSerilog(Log.Logger);

var app = builder.Build();

app.Run();