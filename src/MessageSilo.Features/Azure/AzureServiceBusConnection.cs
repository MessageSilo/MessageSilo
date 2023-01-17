using Azure.Messaging.ServiceBus;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;

namespace MessageSilo.Features.Azure
{
    public class AzureServiceBusConnection : MessagePlatformConnection
    {
        private ServiceBusClient client;

        private ServiceBusReceiver deadLetterReceiver;

        private const int MAX_NUMBER_OF_MESSAGES = 100;

        public string QueueName { get; }

        public string TopicName { get; }

        public string SubscriptionName { get; }

        public AzureServiceBusConnection(string connectionString, string queueName)
        {
            ConnectionString = connectionString;
            QueueName = queueName;
            Type = MessagePlatformType.Azure_Queue;
        }

        public AzureServiceBusConnection(string connectionString, string topicName, string subscriptionName)
        {
            ConnectionString = connectionString;
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

        public override async Task<IEnumerable<Message>> GetDeadLetterMessagesAsync()
        {
            var msgs = await deadLetterReceiver.PeekMessagesAsync(MAX_NUMBER_OF_MESSAGES);

            return msgs.Select(p => new Message(p.MessageId, p.EnqueuedTime, p.Body.ToString()));
        }

        public override async ValueTask DisposeAsync()
        {
            if (deadLetterReceiver is not null)
                await deadLetterReceiver.DisposeAsync();

            if (client is not null)
                await client.DisposeAsync();
        }
    }
}
