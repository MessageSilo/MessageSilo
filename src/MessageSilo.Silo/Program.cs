using MessageSilo.Features.DeadLetterCorrector;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Grains;
using Orleans;
using Orleans.Hosting;

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
});

var app = builder.Build();

app.Start();

var grainFactory = app.Services.GetRequiredService<IGrainFactory>();

var testUserGrain = grainFactory.GetGrain<IUserGrain>("test");

await testUserGrain.AddDeadLetterCorrector(new ConnectionSettingsDTO()
{
    Id = Guid.Parse("c8e2bc0e-8c39-4df2-9003-2558279fcf3d"),
    Name = "test1",
    QueueName = "test_queue",
    ConnectionString = "Endpoint=sb://messagesilo-poc.servicebus.windows.net/;SharedAccessKeyName=qr;SharedAccessKey=4clFdunVfT+UEl5cDlOtjvFD15WD0uRHherkuqMAzAA=;EntityPath=test_queue",
    Type = MessagePlatformType.Azure_Queue,
    CorrectorFuncBody = "(x) => { return {...x, 'newProp':'yeee'}; }"
});

Console.ReadKey();

