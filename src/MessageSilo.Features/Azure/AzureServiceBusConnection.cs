using Azure.Messaging.ServiceBus;
using CsvHelper;
using MessageSilo.Features.MessageCorrector;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;
using System;

namespace MessageSilo.Features.Azure
{
    public class AzureServiceBusConnection : MessagePlatformConnection
    {
        private ServiceBusClient client;

        private ServiceBusProcessor deadLetterProcessor;

        private ServiceBusSender sender;

        private readonly ILogger logger;

        private const int MAX_NUMBER_OF_MESSAGES = 100;

        public string QueueName { get; }

        public string TopicName { get; }

        public string SubscriptionName { get; }

        public AzureServiceBusConnection(string connectionString, string queueName, ILogger logger)
        {
            ConnectionString = connectionString;
            QueueName = queueName;
            Type = MessagePlatformType.Azure_Queue;
            this.logger = logger;
        }

        public AzureServiceBusConnection(string connectionString, string topicName, string subscriptionName, ILogger logger)
        {
            ConnectionString = connectionString;
            TopicName = topicName;
            SubscriptionName = subscriptionName;
            Type = MessagePlatformType.Azure_Topic;
            this.logger = logger;
        }

        public override async Task InitDeadLetterCorrector()
        {
            client = new ServiceBusClient(ConnectionString);

            switch (Type)
            {
                case MessagePlatformType.Azure_Queue:
                    deadLetterProcessor = client.CreateProcessor(QueueName, new ServiceBusProcessorOptions() { SubQueue = SubQueue.DeadLetter, ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });
                    sender = client.CreateSender(QueueName);
                    break;
                case MessagePlatformType.Azure_Topic:
                    deadLetterProcessor = client.CreateProcessor(TopicName, SubscriptionName, new ServiceBusProcessorOptions() { SubQueue = SubQueue.DeadLetter, ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });
                    sender = client.CreateSender(TopicName);
                    break;
            }

            deadLetterProcessor.ProcessMessageAsync += processMessageAsync;
            deadLetterProcessor.ProcessErrorAsync += processErrorAsync;
            await deadLetterProcessor.StartProcessingAsync();
        }

        private Task processErrorAsync(ProcessErrorEventArgs arg)
        {
            logger.LogError(arg.Exception, "deadLetterProcessor");
            return Task.CompletedTask;
        }

        private Task processMessageAsync(ProcessMessageEventArgs arg)
        {
            OnMessageReceived(new MessageReceivedEventArgs(new Message(arg.Message.MessageId, arg.Message.EnqueuedTime, arg.Message.Body.ToString())));
            return Task.CompletedTask;
        }

        public override async Task Enqueue(string msgBody)
        {
            var msg = new ServiceBusMessage(msgBody);
            msg.ApplicationProperties.Add("IsHandledByMessageSilo", true);

            await sender.SendMessageAsync(msg);
        }

        public override async ValueTask DisposeAsync()
        {
            if (deadLetterProcessor is not null)
                await deadLetterProcessor.DisposeAsync();

            if (sender is not null)
                await sender.DisposeAsync();

            if (client is not null)
                await client.DisposeAsync();
        }
    }
}
