using MessageSilo.Features.AWS;
using MessageSilo.Features.Azure;
using MessageSilo.Features.MessageCorrector;
using MessageSilo.Features.RabbitMQ;
using MessageSilo.Features.Target;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System.Runtime;

namespace MessageSilo.Features.Connection
{
    public class ConnectionGrain : Grain, IConnectionGrain
    {
        private readonly ILogger<ConnectionGrain> logger;

        private readonly IGrainFactory grainFactory;

        private IMessagePlatformConnection messagePlatformConnection;
        private IPersistentState<ConnectionState> persistence { get; set; }

        private IDisposable healthTimer;

        private IMessageSenderGrain? target;

        public ConnectionGrain([PersistentState("ConnectionState")] IPersistentState<ConnectionState> state, ILogger<ConnectionGrain> logger, IGrainFactory grainFactory)
        {
            this.persistence = state;
            this.logger = logger;
            this.grainFactory = grainFactory;
        }

        public override async Task OnActivateAsync()
        {
            this.persistence.State.Status = Status.Created;

            if (this.persistence.RecordExists)
                await reInit();

            if (healthTimer is null)
                healthTimer = RegisterTimer(healthCheck, null, TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(10));

            await base.OnActivateAsync();
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

            if (healthTimer is not null)
                healthTimer.Dispose();

            await this.persistence.ClearStateAsync();
        }

        public async Task<ConnectionState> GetState()
        {
            return await Task.FromResult(persistence.State);
        }

        public async Task Send(string msgBody)
        {
            await messagePlatformConnection.Enqueue(msgBody);
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
                        messagePlatformConnection = new AzureServiceBusConnection(settings.ConnectionString, settings.QueueName, settings.SubQueue, logger);
                        break;
                    case MessagePlatformType.Azure_Topic:
                        messagePlatformConnection = new AzureServiceBusConnection(settings.ConnectionString, settings.TopicName, settings.SubscriptionName, settings.SubQueue, logger);
                        break;
                    case MessagePlatformType.RabbitMQ:
                        messagePlatformConnection = new RabbitMQConnection(settings.ConnectionString, settings.QueueName, settings.ExchangeName, settings.AutoAck, logger);
                        break;
                    case MessagePlatformType.AWS_SQS:
                        messagePlatformConnection = new AWSSQSConnection(settings.QueueName, settings.Region, settings.AccessKey, settings.SecretAccessKey, settings.AutoAck, logger);
                        break;
                }

                messagePlatformConnection.MessageReceived += messageReceived;

                await messagePlatformConnection.Init();

                persistence.State.Status = Status.Connected;
            }
            catch (Exception ex)
            {
                persistence.State.Status = Status.Error;
                logger.LogError(ex.Message);
            }
        }

        private void messageReceived(object? sender, EventArgs e)
        {
            var msg = (e as MessageReceivedEventArgs)!.Message;

            logger.Debug($"Connection: {this.GetPrimaryKeyString()} received a messsage: {msg.Id}.");

            var messageCorrector = grainFactory.GetGrain<IMessageCorrectorGrain>($"{this.GetPrimaryKeyString()}|corrector");

            messageCorrector.InvokeOneWay(p => p.CorrectMessage(this, msg, target));
        }

        private Task healthCheck(object state)
        {
            logger.Info($"Connection: {this.GetPrimaryKeyString()} is healthy.");
            return Task.CompletedTask;
        }
    }
}
