using Azure.Messaging.ServiceBus;
using CsvHelper;
using MessageSilo.Features.MessageCorrector;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;
using System;
using SQ = Azure.Messaging.ServiceBus.SubQueue;

namespace MessageSilo.Features.Azure
{
    public class AzureServiceBusConnection : MessagePlatformConnection
    {
        private ServiceBusClient client;

        private ServiceBusProcessor processor;

        private ServiceBusSender sender;

        private readonly ILogger logger;

        public string QueueName { get; }

        public string TopicName { get; }

        public string SubscriptionName { get; }

        public string SubQueue { get; }

        private SQ sq => SubQueue == "DeadLetter" ? SQ.DeadLetter : SQ.None;

        public AzureServiceBusConnection(string connectionString, string queueName, string subQueue, ILogger logger)
        {
            ConnectionString = connectionString;
            QueueName = queueName;
            SubQueue = subQueue;
            Type = MessagePlatformType.Azure_Queue;
            this.logger = logger;
        }

        public AzureServiceBusConnection(string connectionString, string topicName, string subscriptionName, string subQueue, ILogger logger)
        {
            ConnectionString = connectionString;
            TopicName = topicName;
            SubscriptionName = subscriptionName;
            SubQueue = subQueue;
            Type = MessagePlatformType.Azure_Topic;
            this.logger = logger;
        }

        public override async Task Init()
        {
            client = new ServiceBusClient(ConnectionString);

            switch (Type)
            {
                case MessagePlatformType.Azure_Queue:
                    processor = client.CreateProcessor(QueueName, new ServiceBusProcessorOptions() { SubQueue = sq, ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });
                    sender = client.CreateSender(QueueName);
                    break;
                case MessagePlatformType.Azure_Topic:
                    processor = client.CreateProcessor(TopicName, SubscriptionName, new ServiceBusProcessorOptions() { SubQueue = sq, ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });
                    sender = client.CreateSender(TopicName);
                    break;
            }

            processor.ProcessMessageAsync += processMessageAsync;
            processor.ProcessErrorAsync += processErrorAsync;
            await processor.StartProcessingAsync();
        }

        private Task processErrorAsync(ProcessErrorEventArgs arg)
        {
            logger.LogError(arg.Exception, "deadLetterProcessor");
            return Task.CompletedTask;
        }

        private Task processMessageAsync(ProcessMessageEventArgs arg)
        {
            OnMessageReceived(new MessageReceivedEventArgs(new Message(arg.Message.MessageId, arg.Message.Body.ToString())));
            return Task.CompletedTask;
        }

        public override async Task Enqueue(string msgBody)
        {
            try
            {
                var msg = new ServiceBusMessage(msgBody);
                await sender.SendMessageAsync(msg);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
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
