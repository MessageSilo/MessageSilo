using MessageSilo.Features.AWS;
using MessageSilo.Features.Azure;
using MessageSilo.Features.Enricher;
using MessageSilo.Features.EntityManager;
using MessageSilo.Features.Hubs;
using MessageSilo.Features.RabbitMQ;
using MessageSilo.Features.Target;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Extensions;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MessageSilo.Features.Connection
{
    public class ConnectionGrain : Grain, IConnectionGrain
    {
        private readonly ILogger<ConnectionGrain> logger;

        private readonly IGrainFactory grainFactory;

        private readonly IHubContext<SignalHub> hubContext;

        private string? targetId { get; set; }

        private EntityKind? targetKind { get; set; }

        private MessagePlatformType? messagePlatformType { get; set; }

        private List<string> enrichers = new List<string>();

        public ConnectionGrain(ILogger<ConnectionGrain> logger, IGrainFactory grainFactory, IHubContext<SignalHub> hubContext)
        {
            this.logger = logger;
            this.grainFactory = grainFactory;
            this.hubContext = hubContext;
        }

        public async Task Init(bool force = false)
        {
            if (!force && messagePlatformType is not null)
                return;

            var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();

            await hubContext.Clients.Group(userId).SendAsync("signalReceived", new Signal($"{name}#{scaleSet}", SignalType.Initializing, LogLevel.None, "Initialization started..."));

            try
            {
                var em = grainFactory.GetGrain<IEntityManagerGrain>(userId);
                var settings = await em.GetConnectionSettings(name);

                if (settings == null)
                    return;

                if (settings.TargetId is not null)
                {
                    targetId = $"{settings.TargetId}#{scaleSet}";
                    targetKind = settings.TargetKind;
                }

                messagePlatformType = settings.Type.Value;
                enrichers = settings.Enrichers.ToList();
                var messagePlatformConnection = getMessagePlatformConnection();
                await messagePlatformConnection.Init(settings);

                await hubContext.Clients.Group(userId).SendAsync("signalReceived", new Signal($"{name}#{scaleSet}", SignalType.Active, LogLevel.Information, "Active"));
            }
            catch (Exception ex)
            {
                var msg = $"[Connection][{name}#{scaleSet}] Initialization error - {ex.Message}";
                logger.LogError(ex, msg);
                await hubContext.Clients.Group(userId).SendAsync("signalReceived", new Signal($"{name}#{scaleSet}", SignalType.Malfunctioned, LogLevel.Error, msg));
            }
        }

        public async Task Delete()
        {
            var messagePlatformConnection = getMessagePlatformConnection();
            await messagePlatformConnection.DisposeAsync();
        }

        public async Task Send(Message message)
        {
            try
            {
                await Init();

                var messagePlatformConnection = getMessagePlatformConnection();
                await messagePlatformConnection.Enqueue(message);
            }
            catch (Exception ex)
            {
                var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();
                var msg = $"[Connection][{name}#{scaleSet}] Cannot enqueue message [{message?.Id}] - {ex.Message}";
                logger.LogError(ex, msg);
                await hubContext.Clients.Group(userId).SendAsync("signalReceived", new Signal($"{name}#{scaleSet}", SignalType.Malfunctioned, LogLevel.Error, msg));
                throw;
            }
        }

        public async Task<bool> TransformAndSend(Message message)
        {
            try
            {
                await Init();

                var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();

                foreach (var enricherName in enrichers)
                {
                    if (message is null)
                        return false;

                    var enricherGrain = grainFactory.GetGrain<IEnricherGrain>($"{userId}|{enricherName}#{scaleSet}");

                    message = await enricherGrain.Enrich(message);
                }

                if (message is null)
                    return false;

                if (targetId is not null)
                    await getTarget().Send(message);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task Health()
        {

        }

        private IMessageSenderGrain getTarget()
        {
            return targetKind switch
            {
                EntityKind.Connection => grainFactory.GetGrain<IConnectionGrain>(targetId),
                EntityKind.Target => grainFactory.GetGrain<ITargetGrain>(targetId),
                _ => throw new NotSupportedException(),
            };
        }

        private IMessagePlatformConnectionGrain getMessagePlatformConnection()
        {
            return messagePlatformType switch
            {
                MessagePlatformType.Azure_Queue => grainFactory.GetGrain<IAzureServiceBusConnectionGrain>(this.GetPrimaryKeyString()),
                MessagePlatformType.Azure_Topic => grainFactory.GetGrain<IAzureServiceBusConnectionGrain>(this.GetPrimaryKeyString()),
                MessagePlatformType.RabbitMQ => grainFactory.GetGrain<IRabbitMQConnectionGrain>(this.GetPrimaryKeyString()),
                MessagePlatformType.AWS_SQS => grainFactory.GetGrain<IAWSSQSConnectionGrain>(this.GetPrimaryKeyString()),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
