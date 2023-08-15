using MessageSilo.Features.AWS;
using MessageSilo.Features.Azure;
using MessageSilo.Features.Enricher;
using MessageSilo.Features.EntityManager;
using MessageSilo.Features.RabbitMQ;
using MessageSilo.Features.Target;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace MessageSilo.Features.Connection
{
    public class ConnectionGrain : Grain, IConnectionGrain
    {
        private readonly ILogger<ConnectionGrain> logger;

        private readonly IGrainFactory grainFactory;

        private readonly IConfiguration configuration;

        private IMessagePlatformConnection messagePlatformConnection;
        private IPersistentState<ConnectionState> persistence { get; set; }

        private IMessageSenderGrain? target;

        private IEntityManagerGrain entityManager;

        private LastMessage lastMessage;

        public ConnectionGrain([PersistentState("ConnectionState")] IPersistentState<ConnectionState> state, ILogger<ConnectionGrain> logger, IGrainFactory grainFactory, IConfiguration configuration)
        {
            this.persistence = state;
            this.logger = logger;
            this.grainFactory = grainFactory;
            this.configuration = configuration;
        }

        public override async Task OnActivateAsync()
        {
            this.persistence.State.Status = Status.Created;
            this.persistence.State.InitializationError = null;

            if (this.persistence.RecordExists)
                await reInit();

            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            var grain = grainFactory.GetGrain<IConnectionGrain>(this.GetPrimaryKeyString());

            await grain.GetState();
        }

        public async Task Update(ConnectionSettingsDTO s)
        {
            await s.Encrypt(configuration["Orleans:StateUnlockerKey"]);
            persistence.State.ConnectionSettings = s;
            await persistence.WriteStateAsync();
            await reInit();
            await persistence.WriteStateAsync();
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

        public async Task<LastMessage> GetLastMessage()
        {
            return await Task.FromResult(lastMessage);
        }

        public async Task Send(Message message)
        {
            entityManager.InvokeOneWay(p => p.IncreaseUsedThroughput(message.Body));
            await messagePlatformConnection.Enqueue(message);
        }

        public async Task TransformAndSend(Message message)
        {
            var settings = persistence.State.ConnectionSettings;

            foreach (var enricherName in settings.Enrichers)
            {
                var enricherGrain = grainFactory.GetGrain<IEnricherGrain>($"{settings.PartitionKey}|{enricherName}");

                message = await enricherGrain.Enrich(message);

                if (message is null)
                    break;

                entityManager.InvokeOneWay(p => p.IncreaseUsedThroughput(message.Body));
            }

            lastMessage.SetOutput(message, null);

            target?.InvokeOneWay(p => p.Send(message));
        }

        private async Task reInit()
        {
            try
            {
                var settings = persistence.State.ConnectionSettings;
                await settings.Decrypt(configuration["Orleans:StateUnlockerKey"]);

                entityManager = grainFactory.GetGrain<IEntityManagerGrain>(settings.PartitionKey);


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
                        messagePlatformConnection = new AzureServiceBusConnection(settings.ConnectionString, settings.QueueName, settings.SubQueue, settings.ReceiveMode!.Value, logger);
                        break;
                    case MessagePlatformType.Azure_Topic:
                        messagePlatformConnection = new AzureServiceBusConnection(settings.ConnectionString, settings.TopicName, settings.SubscriptionName, settings.SubQueue, settings.ReceiveMode!.Value, logger);
                        break;
                    case MessagePlatformType.RabbitMQ:
                        messagePlatformConnection = new RabbitMQConnection(settings.ConnectionString, settings.QueueName, settings.ExchangeName, settings.ReceiveMode!.Value, logger);
                        break;
                    case MessagePlatformType.AWS_SQS:
                        messagePlatformConnection = new AWSSQSConnection(settings.QueueName, settings.Region, settings.AccessKey, settings.SecretAccessKey, settings.ReceiveMode!.Value, logger);
                        break;
                }

                messagePlatformConnection.MessageReceived += messageReceived;

                await messagePlatformConnection.Init();

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

        private void messageReceived(object? sender, EventArgs e)
        {
            var msg = (e as MessageReceivedEventArgs)!.Message;

            logger.Debug($"Connection: {this.GetPrimaryKeyString()} received a messsage: {msg.Id}.");

            lastMessage = new LastMessage(msg);

            grainFactory.GetGrain<IConnectionGrain>(this.GetPrimaryKeyString()).InvokeOneWay(p => p.TransformAndSend(msg));
        }
    }
}
