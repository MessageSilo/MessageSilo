using MessageSilo.Shared.Enums;

namespace MessageSilo.Shared.Platforms
{
    public class AWSPlatform : IPlatform
    {
        public string Id => "aws";

        public string Title => "Amazon Simple Queue Service";

        public List<MessagePlatformType> Types => new List<MessagePlatformType>() { MessagePlatformType.AWS_SQS };
    }
}
