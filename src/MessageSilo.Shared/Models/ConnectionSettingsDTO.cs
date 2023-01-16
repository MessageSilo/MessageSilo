using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Platforms;

namespace MessageSilo.Shared.Models
{
    public class ConnectionSettingsDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public string PlatformId { get; set; } = new AzurePlatform().Id;

        public MessagePlatformType Type { get; set; } = MessagePlatformType.Azure_Queue;

        public string QueueName { get; set; }

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }

        public string CorrectorFuncBody { get; set; }
    }
}
