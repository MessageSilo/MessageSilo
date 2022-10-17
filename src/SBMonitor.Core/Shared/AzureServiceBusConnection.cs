﻿using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;
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

        private ServiceBusReceiver deadLetterReceiver;

        private const int MAX_NUMBER_OF_MESSAGES = 100;

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

        public override void InitDeadLetterCorrector()
        {
            client = new ServiceBusClient(ConnectionString);

            switch (Type)
            {
                case MessagePlatformType.Azure_Queue:
                    deadLetterReceiver = client.CreateReceiver(QueueName, new ServiceBusReceiverOptions() { SubQueue = SubQueue.DeadLetter });
                    break;
                case MessagePlatformType.Azure_Topic:
                    deadLetterReceiver = client.CreateReceiver(TopicName, SubscriptionName, new ServiceBusReceiverOptions() { SubQueue = SubQueue.DeadLetter });
                    break;
            }
        }

        public override async Task<IEnumerable<string>> GetDeadLetterMessagesAsync()
        {
            var msgs = await deadLetterReceiver.PeekMessagesAsync(MAX_NUMBER_OF_MESSAGES);

            return msgs.Select(p => p.Body.ToString());
        }
    }
}
