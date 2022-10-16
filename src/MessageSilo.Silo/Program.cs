using Orleans;
using Orleans.Hosting;
using SBMonitor.Infrastructure.User;

var builder = Host.CreateDefaultBuilder(args)
        .UseOrleans(siloBuilder => siloBuilder
        .UseLocalhostClustering());

var app = builder.Build();

var grainFactory = app.Services.GetRequiredService<IGrainFactory>();

var testUserGrain = grainFactory.GetGrain<IUserGrain>("berkid89@gmail.com");

testUserGrain.InitDeadLetterCorrectors();

app.Run();