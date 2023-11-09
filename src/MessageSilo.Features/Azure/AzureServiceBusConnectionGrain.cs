﻿using Azure.Messaging.ServiceBus;
using MessageSilo.Features.AWS;
using MessageSilo.Features.Connection;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;
using SQ = Azure.Messaging.ServiceBus.SubQueue;

namespace MessageSilo.Features.Azure
{
    public class AzureServiceBusConnectionGrain : MessagePlatformConnectionGrain, IAzureServiceBusConnectionGrain
    {
        private readonly ILogger<AzureServiceBusConnectionGrain> logger;

        private readonly IGrainFactory grainFactory;

        private ServiceBusClient client;

        private ServiceBusProcessor processor;

        private ServiceBusSender sender;

        public AzureServiceBusConnectionGrain(ILogger<AzureServiceBusConnectionGrain> logger, IGrainFactory grainFactory)
        {
            this.logger = logger;
            this.grainFactory = grainFactory;
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken token)
        {
            var grain = grainFactory.GetGrain<IAzureServiceBusConnectionGrain>(this.GetPrimaryKeyString());

            grain.GetPrimaryKeyString();

            return Task.CompletedTask;
        }

        public override async Task Init(ConnectionSettingsDTO settings)
        {
            this.settings = settings;
            var sq = this.settings.SubQueue == "DeadLetter" ? SQ.DeadLetter : SQ.None;

            client = new ServiceBusClient(this.settings.ConnectionString);

            var sbrm = this.settings.ReceiveMode == ReceiveMode.ReceiveAndDelete ? ServiceBusReceiveMode.ReceiveAndDelete : ServiceBusReceiveMode.PeekLock;

            switch (this.settings.Type)
            {
                case MessagePlatformType.Azure_Queue:
                    processor = client.CreateProcessor(this.settings.QueueName, new ServiceBusProcessorOptions() { SubQueue = sq, ReceiveMode = sbrm, AutoCompleteMessages = this.settings.ReceiveMode == ReceiveMode.ReceiveAndDelete });
                    sender = client.CreateSender(this.settings.QueueName);
                    break;
                case MessagePlatformType.Azure_Topic:
                    processor = client.CreateProcessor(this.settings.TopicName, this.settings.SubscriptionName, new ServiceBusProcessorOptions() { SubQueue = sq, ReceiveMode = sbrm, AutoCompleteMessages = this.settings.ReceiveMode == ReceiveMode.ReceiveAndDelete });
                    sender = client.CreateSender(this.settings.TopicName);
                    break;
            }

            if (this.settings.ReceiveMode != ReceiveMode.None)
            {
                processor.ProcessMessageAsync += processMessageAsync;
                processor.ProcessErrorAsync += processErrorAsync;
                await processor.StartProcessingAsync();
            }
        }

        private async Task processErrorAsync(ProcessErrorEventArgs arg)
        {
            logger.LogError(arg.Exception, arg.Exception.Message);
        }

        private async Task processMessageAsync(ProcessMessageEventArgs arg)
        {
            var connection = grainFactory.GetGrain<IConnectionGrain>(this.GetPrimaryKeyString());

            await connection.TransformAndSend(new Message(arg.Message.MessageId, arg.Message.Body.ToString()));
        }

        public override async Task Enqueue(Message message)
        {
            var msg = new ServiceBusMessage(message.Body);
            await sender.SendMessageAsync(msg);
        }

        public override async ValueTask DisposeAsync()
        {
            if (processor is not null)
                await processor.DisposeAsync();

            if (sender is not null)
                await sender.DisposeAsync();

            if (client is not null)
                await client.DisposeAsync();
        }
    }
}
