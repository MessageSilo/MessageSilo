using Serilog.Sinks.ApplicationInsights.TelemetryConverters;
using Serilog;
using MessageSilo.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using MessageSilo.HealthChecks;
using System.Net;
using Orleans.Configuration;
using Orleans.Providers;
using Orleans.Serialization;
using MessageSilo.Shared.Models;

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

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans(siloBuilder =>
{
    var siloIP = IPAddress.Parse(configuration["PrimarySiloAddress"]);

    siloBuilder
    .UseMongoDBClient(configuration["DatabaseConnectionString"])
    .UseMongoDBClustering(options =>
    {
        options.DatabaseName = configuration["DatabaseName"];
        options.CreateShardKeyForCosmos = false;
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

builder.Host.UseSerilog(Log.Logger);

builder.Services.AddControllers();

builder.Services.AddAuthentication()
        .AddScheme<OnboardingAuthSchemeOptions, OnboardingAuthHandler>("OnboardingAuthScheme", options => { })
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
        "OnboardingAuthScheme");
    defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck<ShallowHealthCheck>("shallow")
    .AddCheck<DeepHealthCheck>("deep");

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
