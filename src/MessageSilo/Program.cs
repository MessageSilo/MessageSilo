using MessageSilo;
using MessageSilo.HealthChecks;
using MessageSilo.Infrastructure.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Orleans.Configuration;
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

if (!string.IsNullOrWhiteSpace(configuration["APPINSIGHTS_CONNECTIONSTRING"]))
    loggerConfig.WriteTo.ApplicationInsights(configuration["APPINSIGHTS_CONNECTIONSTRING"], new TraceTelemetryConverter());

Log.Logger = loggerConfig.CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(siloBuilder =>
{
    var siloIP = string.IsNullOrWhiteSpace(configuration["PRIMARY_SILO_ADDRESS"]) ? IPAddress.Loopback : IPAddress.Parse(configuration["PRIMARY_SILO_ADDRESS"]);

    if (!string.IsNullOrWhiteSpace(configuration["DATABASE_CONNECTION_STRING"]) && !string.IsNullOrWhiteSpace(configuration["DATABASE_CONNECTION_STRING"]))
    {
        siloBuilder.UseMongoDBClient(configuration["DATABASE_CONNECTION_STRING"])
        .UseMongoDBClustering(options =>
        {
            options.DatabaseName = configuration["DATABASE_NAME"];
            options.CreateShardKeyForCosmos = false;
        })
        .AddMongoDBGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, options =>
        {
            options.DatabaseName = configuration["DATABASE_NAME"];
        });
    }
    else
    {
        siloBuilder.UseLocalhostClustering(primarySiloEndpoint: new IPEndPoint(siloIP, 11111))
        .AddMemoryGrainStorageAsDefault();
    }

    siloBuilder.Services.AddInfrastructureServices(configuration);

    siloBuilder.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "MessageSiloCluster001";
        options.ServiceId = "MessageSiloService001";
    })
    .ConfigureEndpoints(siloIP, 11111, 30000);
});

builder.Host.UseSerilog(Log.Logger);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHealthChecks()
    .AddCheck<ShallowHealthCheck>("shallow")
    .AddCheck<DeepHealthCheck>("deep");

builder.Services.AddHostedService<StartupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSerilogRequestLogging();
app.UseCors(builder => builder.SetIsOriginAllowed(isOriginAllowed: _ => true).WithExposedHeaders(HeaderNames.ContentDisposition).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapHealthChecks("/health");
app.MapFallbackToFile("/", "index.html");

app.Run();
