using MessageSilo.Shared.Enums;

namespace MessageSilo.Shared.Models
{
    public abstract class MessagePlatformConnection : IMessagePlatformConnection
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public string Name { get; protected set; }

        public string ConnectionString { get; protected set; }

        public MessagePlatformType Type { get; protected set; }

        public abstract void InitDeadLetterCorrector();

        public abstract Task<IEnumerable<Message>> GetDeadLetterMessagesAsync();
    }
}
