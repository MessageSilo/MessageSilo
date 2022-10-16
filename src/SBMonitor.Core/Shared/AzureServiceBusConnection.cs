using Azure.Messaging.ServiceBus;
using SBMonitor.Core.Enums;
using SBMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Shared
{
    public class AzureServiceBusConnection : MessagePlatformConnection
    {
        private ServiceBusClient client;

        private ServiceBusProcessor deadLetterProcessor;

        public string QueueName { get; }

        public string TopicName { get; }

        public string SubscriptionName { get; }

        public AzureServiceBusConnection(string queueName)
        {
            QueueName = queueName;
            Type = MessagePlatformType.Azure_Queue;
        }

        public AzureServiceBusConnection(string topicName, string subscriptionName)
        {
            TopicName = topicName;
            SubscriptionName = subscriptionName;
            Type = MessagePlatformType.Azure_Topic;
        }

        public override void StartProcessingDeadLetterMessages()
        {
            client = new ServiceBusClient(ConnectionString);

            switch (Type)
            {
                case MessagePlatformType.Azure_Queue:
                    deadLetterProcessor = client.CreateProcessor(QueueName, new ServiceBusProcessorOptions() { SubQueue = SubQueue.DeadLetter });
                    break;
                case MessagePlatformType.Azure_Topic:
                    deadLetterProcessor = client.CreateProcessor(TopicName, SubscriptionName, new ServiceBusProcessorOptions() { SubQueue = SubQueue.DeadLetter });
                    break;
            }

            deadLetterProcessor.ProcessMessageAsync += onDeadLetterMessageReceived;
        }
    }
}
