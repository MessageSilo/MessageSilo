using MessageSilo.Features.DeadLetterCorrector;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Grains;
using Orleans;
using Orleans.Hosting;
using MessageSilo.Shared.DataAccess;

var builder = Host.CreateDefaultBuilder(args)
        .UseOrleans(siloBuilder => siloBuilder
        .ConfigureApplicationParts(manager =>
        {
            manager.AddApplicationPart(typeof(IUserGrain).Assembly);
            manager.AddApplicationPart(typeof(IDeadLetterCorrectorGrain).Assembly);
        })
        .UseLocalhostClustering()
        .AddMemoryGrainStorageAsDefault());

builder.ConfigureServices(services =>
{
    services.AddScoped<IMessageCorrector, MessageCorrector>();
    services.AddSingleton<IMessageRepository<CorrectedMessage>, MessageRepository<CorrectedMessage>>();
});

var app = builder.Build();

app.Start();

Console.ReadKey();

