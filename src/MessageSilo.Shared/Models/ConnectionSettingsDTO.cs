using MessageSilo.Shared.Enums;

namespace MessageSilo.Shared.Models
{
    public class ConnectionSettingsDTO : Entity
    {
        public string ConnectionString { get; set; }

        public MessagePlatformType? Type { get; set; }

        public string QueueName { get; set; }

        public IEnumerable<string> Enrichers { get; set; } = new List<string>();

        public string Target { get; set; }

        public string TargetId => string.IsNullOrEmpty(Target) ? null! : $"{PartitionKey}|{Target}";

        public EntityKind TargetKind { get; set; }

        public bool AutoAck { get; set; }

        //Azure

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }

        public string SubQueue { get; set; }

        //RabbitMQ

        public string ExchangeName { get; set; }

        //AWS

        public string Region { get; set; }

        public string AccessKey { get; set; }

        public string SecretAccessKey { get; set; }

        public ConnectionSettingsDTO()
        {
            Kind = EntityKind.Connection;
        }
    }
}
