using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;

namespace MessageSilo.Application.DTOs
{
    public class ConnectionSettingsDTO : Entity
    {
        //Common
        public string ConnectionString { get; set; }

        public MessagePlatformType? Type { get; set; }

        public string QueueName { get; set; }

        public IEnumerable<string> Enrichers { get; set; } = [];

        public string Target { get; set; }

        public string TargetId => string.IsNullOrEmpty(Target) ? null! : $"{UserId}|{Target}";

        public EntityKind TargetKind { get; set; }

        public ReceiveMode? ReceiveMode { get; set; }

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
