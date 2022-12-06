using MessageSilo.Shared.Enums;

namespace MessageSilo.Shared.Models
{
    public class ConnectionSettingsDTO
    {
        public Guid Id { get; set; } = Guid.Empty;

        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public MessagePlatformType Type { get; set; }

        public string QueueName { get; set; }

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }

        public string CorrectorFuncBody { get; set; }
    }
}
