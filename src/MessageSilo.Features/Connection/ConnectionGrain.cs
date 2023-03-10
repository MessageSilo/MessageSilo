using MessageSilo.Features.Azure;
using MessageSilo.Features.MessageCorrector;
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

        private IDisposable timer;

        private IConnectionGrain? targetConnection;

        public ConnectionGrain([PersistentState("ConnectionState")] IPersistentState<ConnectionState> state, ILogger<ConnectionGrain> logger, IGrainFactory grainFactory)
        {
            this.persistence = state;
            this.logger = logger;
            this.grainFactory = grainFactory;
        }

        public override Task OnActivateAsync()
        {
            if (this.persistence.RecordExists)
                reInit();

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
            timer?.Dispose();

            if (messagePlatformConnection is not null)
                await messagePlatformConnection.DisposeAsync();

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
            if (persistence.State.ConnectionSettings.TargetId is not null)
                targetConnection = grainFactory.GetGrain<ConnectionGrain>(persistence.State.ConnectionSettings.TargetId);

            switch (persistence.State.ConnectionSettings.Type)
            {
                case MessagePlatformType.Azure_Queue:
                    messagePlatformConnection = new AzureServiceBusConnection(persistence.State.ConnectionSettings.ConnectionString, persistence.State.ConnectionSettings.QueueName);
                    break;
                case MessagePlatformType.Azure_Topic:
                    messagePlatformConnection = new AzureServiceBusConnection(persistence.State.ConnectionSettings.ConnectionString, persistence.State.ConnectionSettings.TopicName, persistence.State.ConnectionSettings.SubscriptionName);
                    break;
            }

            messagePlatformConnection!.InitDeadLetterCorrector();

            if (timer is null)
                timer = RegisterTimer(processMessagesAsync, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
        }

        private async Task processMessagesAsync(object state)
        {
            var msgs = await messagePlatformConnection.GetDeadLetterMessagesAsync(persistence.State.LastProcessedMessageSequenceNumber);

            logger.Debug($"Connection: {persistence.State.ConnectionSettings.Id} received {msgs.Count()} dead-lettered messsages.");

            if (msgs.Count() == 0)
                return;

            persistence.State.LastProcessedMessageSequenceNumber = msgs.Max(msg => msg.SequenceNumber);

            var messageCorrector = grainFactory.GetGrain<IMessageCorrectorGrain>(Guid.NewGuid());

            messageCorrector.InvokeOneWay(p => p.CorrectMessages(this, msgs.ToList(), targetConnection));
        }
    }
}
