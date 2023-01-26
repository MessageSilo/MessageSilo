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

namespace MessageSilo.Features.DeadLetterCorrector
{
    public class DeadLetterCorrectorGrain : Grain, IDeadLetterCorrectorGrain
    {
        private readonly IMessageCorrector messageCorrector;
        private readonly IMessageRepository<CorrectedMessage> messages;
        private readonly ILogger<DeadLetterCorrectorGrain> logger;

        private IMessagePlatformConnection messagePlatformConnection;
        private IPersistentState<ConnectionSettingsDTO> setting { get; set; }

        private IDisposable timer;

        public DeadLetterCorrectorGrain([PersistentState("Setting")] IPersistentState<ConnectionSettingsDTO> setting, IMessageCorrector messageCorrector, IMessageRepository<CorrectedMessage> messages)
        {
            this.setting = setting;
            this.messageCorrector = messageCorrector;
            this.messages = messages;
        }

        public override Task OnActivateAsync()
        {
            if (this.setting.RecordExists)
                reInit();

            return base.OnActivateAsync();
        }

        public async Task Update(ConnectionSettingsDTO s)
        {
            setting.State = s;
            await setting.WriteStateAsync();
            reInit();
        }

        public async Task Delete()
        {
            timer?.Dispose();

            if (messagePlatformConnection is not null)
                await messagePlatformConnection.DisposeAsync();

            await this.setting.ClearStateAsync();
        }

        private void reInit()
        {
            switch (setting.State.Type)
            {
                case MessagePlatformType.Azure_Queue:
                    messagePlatformConnection = new AzureServiceBusConnection(setting.State.ConnectionString, setting.State.QueueName);
                    break;
                case MessagePlatformType.Azure_Topic:
                    messagePlatformConnection = new AzureServiceBusConnection(setting.State.ConnectionString, setting.State.TopicName, setting.State.SubscriptionName);
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

            foreach (var msg in msgs)
            {
                string? correctedMessageBody = messageCorrector.Correct(msg.Body, setting.State.CorrectorFuncBody);

                messages.Add(setting.State.Id.ToString(), new CorrectedMessage(msg)
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
