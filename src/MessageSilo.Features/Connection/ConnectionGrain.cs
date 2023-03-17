using MessageSilo.Features.Azure;
using MessageSilo.Features.MessageCorrector;
using MessageSilo.Features.RabbitMQ;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace MessageSilo.Features.Connection
{
    public class ConnectionGrain : Grain, IConnectionGrain
    {
        private readonly ILogger<ConnectionGrain> logger;

        private readonly IGrainFactory grainFactory;

        private IMessagePlatformConnection messagePlatformConnection;
        private IPersistentState<ConnectionState> persistence { get; set; }

        private IConnectionGrain? targetConnection;

        private IDisposable healthTimer;

        public ConnectionGrain([PersistentState("ConnectionState")] IPersistentState<ConnectionState> state, ILogger<ConnectionGrain> logger, IGrainFactory grainFactory)
        {
            this.persistence = state;
            this.logger = logger;
            this.grainFactory = grainFactory;
        }

        public override Task OnActivateAsync()
        {
            this.persistence.State.Status = ConnectionStatus.Created;

            if (this.persistence.RecordExists)
                reInit();

            if (healthTimer is null)
                healthTimer = RegisterTimer(healthCheck, null, TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(10));

            return base.OnActivateAsync();
        }

        public async Task Update(ConnectionSettingsDTO s)
        {
            persistence.State.ConnectionSettings = s;
            await persistence.WriteStateAsync();
            reInit();
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

        public async Task Enqueue(string msgBody)
        {
            await messagePlatformConnection.Enqueue(msgBody);
        }

        private void reInit()
        {
            try
            {
                if (persistence.State.ConnectionSettings.TargetId is not null)
                    targetConnection = grainFactory.GetGrain<IConnectionGrain>(persistence.State.ConnectionSettings.TargetId);

                switch (persistence.State.ConnectionSettings.Type)
                {
                    case MessagePlatformType.Azure_Queue:
                        messagePlatformConnection = new AzureServiceBusConnection(persistence.State.ConnectionSettings.ConnectionString, persistence.State.ConnectionSettings.QueueName, logger);
                        break;
                    case MessagePlatformType.Azure_Topic:
                        messagePlatformConnection = new AzureServiceBusConnection(persistence.State.ConnectionSettings.ConnectionString, persistence.State.ConnectionSettings.TopicName, persistence.State.ConnectionSettings.SubscriptionName, logger);
                        break;
                    case MessagePlatformType.RabbitMQ:
                        messagePlatformConnection = new RabbitMQConnection(persistence.State.ConnectionSettings.ConnectionString, persistence.State.ConnectionSettings.QueueName, persistence.State.ConnectionSettings.ExchangeName, logger);
                        break;
                }

                messagePlatformConnection.MessageReceived += messageReceived;

                messagePlatformConnection.InitDeadLetterCorrector();

                persistence.State.Status = ConnectionStatus.Connected;
            }
            catch (Exception ex)
            {
                persistence.State.Status = ConnectionStatus.Error;
                logger.LogError(ex.Message);
            }
        }

        private void messageReceived(object? sender, EventArgs e)
        {
            var msg = (e as MessageReceivedEventArgs)!.Message;

            logger.Debug($"Connection: {this.GetPrimaryKeyString()} received a dead-lettered messsage: {msg.Id}.");

            var messageCorrector = grainFactory.GetGrain<IMessageCorrectorGrain>($"{this.GetPrimaryKeyString()}|corrector");

            messageCorrector.InvokeOneWay(p => p.CorrectMessage(this, msg, targetConnection));
        }

        private Task healthCheck(object state)
        {
            logger.Info($"Connection: {this.GetPrimaryKeyString()} is healthy.");
            return Task.CompletedTask;
        }
    }
}
