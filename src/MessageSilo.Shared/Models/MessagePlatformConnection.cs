using MessageSilo.Shared.Enums;

namespace MessageSilo.Shared.Models
{
    public abstract class MessagePlatformConnection : IMessagePlatformConnection
    {
        public string Name { get; protected set; }

        public string ConnectionString { get; protected set; }

        public MessagePlatformType Type { get; protected set; }

        public abstract void InitDeadLetterCorrector();

        public abstract Task<IEnumerable<Message>> GetDeadLetterMessagesAsync(long? lastProcessedMessageSequenceNumber);

        public abstract Task Enqueue(string msgBody);

        public abstract ValueTask DisposeAsync();
    }
}
