using MessageSilo.Features.Connection;
using MessageSilo.Features.Enricher;
using MessageSilo.Features.EntityManager;
using MessageSilo.Features.Target;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Models;
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
            siloBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IEntityRepository>(new EntityRepository(configuration));
            });

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
                manager.AddApplicationPart(typeof(IEnricherGrain).Assembly);
                manager.AddApplicationPart(typeof(IEntityManagerGrain).Assembly);
            })
            .AddAzureTableGrainStorageAsDefault(options =>
            {
                options.ConfigureTableServiceClient(configuration["ConnectionStrings:GrainStateStorage"]);
                options.UseJson = true;
                options.DeleteStateOnClear = true;
            });
        });

builder.UseSerilog(Log.Logger);

var app = builder.Build();

app.Run();