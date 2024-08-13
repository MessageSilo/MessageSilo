using System.ComponentModel;

namespace MessageSilo.Domain.Enums
{
    public enum MessagePlatformType
    {
        [Description("Queue")]
        Azure_Queue,

        [Description("Topic")]
        Azure_Topic,

        [Description("SQS")]
        AWS_SQS,

        [Description("RabbitMQ")]
        RabbitMQ
    }
}
