using MessageSilo.Shared.Enums;

namespace MessageSilo.Shared.Platforms
{
    public class AzurePlatform : IPlatform
    {
        public string Id => "azure";

        public string DisplayName => "Azure Service Bus";

        public List<MessagePlatformType> Types => new List<MessagePlatformType>() { MessagePlatformType.Azure_Queue, MessagePlatformType.Azure_Topic };
    }
}
