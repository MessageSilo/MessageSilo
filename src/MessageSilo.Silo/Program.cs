using Orleans;
using Orleans.Hosting;
using SBMonitor.Core.DeadLetterCorrector;
using SBMonitor.Core.Enums;
using SBMonitor.Core.Shared;
using SBMonitor.Infrastructure.User;

var builder = Host.CreateDefaultBuilder(args)
        .UseOrleans(siloBuilder => siloBuilder
        .ConfigureApplicationParts(manager =>
        {
            manager.AddApplicationPart(typeof(SBMonitor.Infrastructure.DeadLetterCorrector.IDeadLetterCorrectorGrain).Assembly);
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
    Name = "test1",
    QueueName = "test_queue",
    ConnectionString = "Endpoint=sb://messagesilo-poc.servicebus.windows.net/;SharedAccessKeyName=qr;SharedAccessKey=4clFdunVfT+UEl5cDlOtjvFD15WD0uRHherkuqMAzAA=;EntityPath=test_queue",
    Type = MessagePlatformType.Azure_Queue,
    CorrectorFuncBody = "(x) => { return {...x, 'newProp':'yeee'}; }"
});

await testUserGrain.InitDeadLetterCorrectors();

Console.ReadKey();

