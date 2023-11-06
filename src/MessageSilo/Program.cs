using MessageSilo;
using MessageSilo.Auth;
using MessageSilo.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
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

    siloBuilder.Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "MessageSiloCluster001";
        options.ServiceId = "MessageSiloService001";
    })
    .ConfigureEndpoints(siloIP, 11111, 30000);
});

builder.Host.UseSerilog(Log.Logger);

builder.Services.AddControllers();

builder.Services.AddAuthentication()
        .AddScheme<LocalAuthSchemeOptions, LocalAuthHandler>("LocalAuthScheme", options => { })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
        {
            c.Authority = $"{configuration["Auth0:Domain"]}";
            c.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = configuration["Auth0:Audience"],
                ValidIssuer = $"{configuration["Auth0:Domain"]}"
            };
        });

builder.Services.AddAuthorization(options =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
        JwtBearerDefaults.AuthenticationScheme,
        "LocalAuthScheme");
    defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck<ShallowHealthCheck>("shallow")
    .AddCheck<DeepHealthCheck>("deep");

builder.Services.AddHostedService<StartupService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseCors(builder => builder.SetIsOriginAllowed(isOriginAllowed: _ => true).WithExposedHeaders(HeaderNames.ContentDisposition).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
