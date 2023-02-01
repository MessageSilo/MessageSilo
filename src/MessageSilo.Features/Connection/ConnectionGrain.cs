using MessageSilo.Features.Azure;
using MessageSilo.Features.User;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System;
using System.Drawing.Imaging;

namespace MessageSilo.Features.Connection
{
    public class ConnectionGrain : Grain, IConnectionGrain
    {
        private readonly IMessageCorrector messageCorrector;
        private readonly IMessageRepository<CorrectedMessage> messages;
        private readonly ILogger<ConnectionGrain> logger;

        private IMessagePlatformConnection messagePlatformConnection;
        private IPersistentState<ConnectionState> persistence { get; set; }

        private IDisposable timer;

        public ConnectionGrain([PersistentState("ConnectionState")] IPersistentState<ConnectionState> state, IMessageCorrector messageCorrector, IMessageRepository<CorrectedMessage> messages)
        {
            this.persistence = state;
            this.messageCorrector = messageCorrector;
            this.messages = messages;
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

        private void reInit()
        {
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
            var msgs = await messagePlatformConnection.GetDeadLetterMessagesAsync();

            if (msgs.Count() == 0)
                return;

            persistence.State.DeadLetteredMessagesCount += msgs.Count();

            foreach (var msg in msgs)
            {
                string? correctedMessageBody = messageCorrector.Correct(msg.Body, persistence.State.ConnectionSettings.CorrectorFuncBody);

                messages.Add(persistence.State.ConnectionSettings.Id.ToString(), new CorrectedMessage(msg)
                {
                    BodyAfterCorrection = correctedMessageBody,
                    IsCorrected = correctedMessageBody != null,
                    IsResent = false,
                });

                //TODO: Auto re-enque message if needed
            }
        }
    }
}
