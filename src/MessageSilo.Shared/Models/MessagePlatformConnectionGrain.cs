using MessageSilo.Domain.Entities;

namespace MessageSilo.Shared.Models
{
    public abstract class MessagePlatformConnectionGrain : Grain, IMessagePlatformConnectionGrain
    {
        protected ConnectionSettingsDTO settings;

        public abstract Task Init(ConnectionSettingsDTO settings);

        public abstract Task Enqueue(Message message);

        public abstract ValueTask DisposeAsync();
    }
}
