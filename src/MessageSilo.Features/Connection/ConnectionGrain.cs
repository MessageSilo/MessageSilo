using MessageSilo.Features.AWS;
using MessageSilo.Features.Azure;
using MessageSilo.Features.Enricher;
using MessageSilo.Features.EntityManager;
using MessageSilo.Features.RabbitMQ;
using MessageSilo.Features.Target;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Extensions;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;

namespace MessageSilo.Features.Connection
{
    /// <summary>
    /// id = UserId|Name#scaleSet
    /// </summary>
    public class ConnectionGrain : Grain, IConnectionGrain
    {
        private readonly ILogger<ConnectionGrain> logger;

        private readonly IGrainFactory grainFactory;

        private string? targetId { get; set; }

        private EntityKind? targetKind { get; set; }

        private MessagePlatformType messagePlatformType { get; set; }

        private List<string> enrichers = new List<string>();

        public ConnectionGrain(ILogger<ConnectionGrain> logger, IGrainFactory grainFactory)
        {
            this.logger = logger;
            this.grainFactory = grainFactory;
        }

        public async Task Init()
        {
            var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();

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
        }

        public async Task Delete()
        {
            var messagePlatformConnection = getMessagePlatformConnection();
            await messagePlatformConnection.DisposeAsync();
        }

        public async Task Send(Message message)
        {
            var messagePlatformConnection = getMessagePlatformConnection();
            await messagePlatformConnection.Enqueue(message);
        }

        public async Task TransformAndSend(Message message)
        {
            var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();

            foreach (var enricherName in enrichers)
            {
                if (message is null)
                    break;

                var enricherGrain = grainFactory.GetGrain<IEnricherGrain>($"{userId}|{enricherName}#{scaleSet}");

                message = await enricherGrain.Enrich(message);
            }

            if (targetId is not null && message is not null)
                await getTarget().Send(message);
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
