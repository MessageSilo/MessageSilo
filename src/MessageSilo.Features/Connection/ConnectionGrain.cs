using MessageSilo.Features.AWS;
using MessageSilo.Features.Azure;
using MessageSilo.Features.Enricher;
using MessageSilo.Features.RabbitMQ;
using MessageSilo.Features.Target;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace MessageSilo.Features.Connection
{
    public class ConnectionGrain : Grain, IConnectionGrain
    {
        private readonly ILogger<ConnectionGrain> logger;

        private readonly IGrainFactory grainFactory;

        private IMessagePlatformConnectionGrain messagePlatformConnection { get; set; }
        private IPersistentState<ConnectionState> persistence { get; set; }

        private IMessageSenderGrain? target { get; set; }

        public ConnectionGrain([PersistentState("ConnectionState")] IPersistentState<ConnectionState> state, ILogger<ConnectionGrain> logger, IGrainFactory grainFactory)
        {
            this.persistence = state;
            this.logger = logger;
            this.grainFactory = grainFactory;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            this.persistence.State.Status = Status.Created;
            this.persistence.State.InitializationError = null;

            if (this.persistence.RecordExists)
                await reInit();

            await base.OnActivateAsync(cancellationToken);
        }

        public async Task Update(ConnectionSettingsDTO s)
        {
            persistence.State.ConnectionSettings = s;
            await persistence.WriteStateAsync();
            await reInit();
        }

        public async Task Delete()
        {
            if (messagePlatformConnection is not null)
                await messagePlatformConnection.DisposeAsync();

            await this.persistence.ClearStateAsync();
        }

        public async Task<ConnectionState> GetState()
        {
            return await Task.FromResult(persistence.State);
        }

        public async Task Send(Message message)
        {
            await messagePlatformConnection.Enqueue(message);
        }

        public async Task TransformAndSend(Message message)
        {
            var settings = persistence.State.ConnectionSettings;

            foreach (var enricherName in settings.Enrichers)
            {
                var enricherGrain = grainFactory.GetGrain<IEnricherGrain>($"{settings.UserId}|{enricherName}");

                message = await enricherGrain.Enrich(message);

                if (message is null)
                    break;
            }

            if (target is not null)
                await target.Send(message);
        }

        private async Task reInit()
        {
            try
            {
                var settings = persistence.State.ConnectionSettings;

                if (settings.TargetId is not null)
                    switch (settings.TargetKind)
                    {
                        case EntityKind.Connection:
                            target = grainFactory.GetGrain<IConnectionGrain>(settings.TargetId);
                            break;
                        case EntityKind.Target:
                            target = grainFactory.GetGrain<ITargetGrain>(settings.TargetId);
                            break;
                    }

                switch (settings.Type)
                {
                    case MessagePlatformType.Azure_Queue:
                    case MessagePlatformType.Azure_Topic:
                        messagePlatformConnection = grainFactory.GetGrain<IAzureServiceBusConnectionGrain>(this.GetPrimaryKeyString());
                        break;
                    case MessagePlatformType.RabbitMQ:
                        messagePlatformConnection = grainFactory.GetGrain<IRabbitMQConnectionGrain>(this.GetPrimaryKeyString());
                        break;
                    case MessagePlatformType.AWS_SQS:
                        messagePlatformConnection = grainFactory.GetGrain<IAWSSQSConnectionGrain>(this.GetPrimaryKeyString());
                        break;
                }

                await messagePlatformConnection.Init(settings);

                persistence.State.Status = Status.Connected;
                persistence.State.InitializationError = null;
            }
            catch (Exception ex)
            {
                persistence.State.Status = Status.Error;
                persistence.State.InitializationError = ex.Message;
                logger.LogError(ex.Message);
            }
        }
    }
}
