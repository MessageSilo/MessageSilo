using MessageSilo.Shared.Enums;

namespace MessageSilo.Shared.Models
{
    public class ConnectionSettingsDTO
    {
        //Common

        public string Token { get; set; }

        public string Name { get; set; }

        public string Id => $"{Token}|{Name}";

        public string ConnectionString { get; set; }

        public MessagePlatformType Type { get; set; }

        public string QueueName { get; set; }

        public string CorrectorFuncBody { get; set; }

        public string Target { get; set; }

        public string TargetId => string.IsNullOrEmpty(Target) ? null! : $"{Token}|{Target}";

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
    }
}
