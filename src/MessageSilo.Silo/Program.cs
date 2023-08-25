using MessageSilo.Features.Connection;
using MessageSilo.Features.Enricher;
using MessageSilo.Features.EntityManager;
using MessageSilo.Features.Target;
using MessageSilo.Features.UserManager;
using MessageSilo.Shared.Models;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers;
using Serilog;
using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using System.Net;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile("appsettings.Development.json", true, true)
    .AddEnvironmentVariables()
    .Build();

var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .WriteTo.Console();

if (!string.IsNullOrWhiteSpace(configuration["AppInsightsConnectionString"]))
    loggerConfig.WriteTo.ApplicationInsights(configuration["AppInsightsConnectionString"], new TraceTelemetryConverter());

Log.Logger = loggerConfig.CreateBootstrapLogger();

var builder = Host.CreateDefaultBuilder(args)
        .UseOrleans(siloBuilder =>
        {
            var siloIP = IPAddress.Parse(configuration["PrimarySiloAddress"]);

            siloBuilder
            .UseMongoDBClient(configuration["DatabaseConnectionString"])
            .UseMongoDBClustering(options =>
            {
                options.DatabaseName = configuration["DatabaseName"];
                options.CreateShardKeyForCosmos = false;
            })
            .ConfigureApplicationParts(manager =>
            {
                manager.AddApplicationPart(typeof(IMessageSenderGrain).Assembly);
                manager.AddApplicationPart(typeof(ITargetGrain).Assembly);
                manager.AddApplicationPart(typeof(IConnectionGrain).Assembly);
                manager.AddApplicationPart(typeof(IEnricherGrain).Assembly);
                manager.AddApplicationPart(typeof(IEntityManagerGrain).Assembly);
                manager.AddApplicationPart(typeof(IUserManagerGrain).Assembly);
            })
            .AddMongoDBGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, options =>
            {
                options.DatabaseName = configuration["DatabaseName"];
            })
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "MessageSiloCluster001";
                options.ServiceId = "MessageSiloService001";
            })
            .ConfigureEndpoints(siloIP, 11111, 30000);
        });

builder.UseSerilog(Log.Logger);

var app = builder.Build();

app.Run();