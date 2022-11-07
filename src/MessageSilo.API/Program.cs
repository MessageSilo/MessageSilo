using MessageSilo.API;
using Orleans;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ClusterClientHostedService>();
builder.Services.AddSingleton<IHostedService>(
            sp => sp.GetService<ClusterClientHostedService>()!);
builder.Services.AddSingleton<IClusterClient>(
            sp => sp.GetService<ClusterClientHostedService>()!.Client);
builder.Services.AddSingleton<IGrainFactory>(
            sp => sp.GetService<ClusterClientHostedService>()!.Client);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();