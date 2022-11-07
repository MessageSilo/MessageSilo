using MessageSilo.Features.Shared.Enums;
using MessageSilo.Features.Shared.Models;

namespace MessageSilo.Features.AWS
{
    public class AWSPlatform : IPlatform
    {
        public string Id => "aws";

        public string DisplayName => "Amazon Simple Queue Service";

        public List<MessagePlatformType> Types => new List<MessagePlatformType>() { MessagePlatformType.AWS_SQS };
    }
}
