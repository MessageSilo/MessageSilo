using FluentValidation;
using MessageSilo.API;
using MessageSilo.API.HealthChecks;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Orleans;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile("appsettings.Development.json", true, true);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSingleton<ClusterClientHostedService>();
builder.Services.AddSingleton<IHostedService>(sp => sp.GetService<ClusterClientHostedService>()!);
builder.Services.AddSingleton<IClusterClient>(sp => sp.GetService<ClusterClientHostedService>()!.Client);
builder.Services.AddSingleton<IGrainFactory>(sp => sp.GetService<ClusterClientHostedService>()!.Client);
builder.Services.AddSingleton<IEntityRepository, EntityRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
        {
            c.Authority = $"{builder.Configuration["Auth0:Domain"]}";
            c.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = builder.Configuration["Auth0:Audience"],
                ValidIssuer = $"{builder.Configuration["Auth0:Domain"]}"
            };
        });

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck<ShallowHealthCheck>("shallow")
    .AddCheck<DeepHealthCheck>("deep");

builder.Host.UseSerilog();

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