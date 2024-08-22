using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;
using MessageSilo.Infrastructure.Interfaces;

namespace MessageSilo.Infrastructure.Services
{
    public abstract class MessagePlatformConnectionGrain : Grain, IMessagePlatformConnectionGrain
    {
        protected ConnectionSettingsDTO settings;

        public abstract Task Init(ConnectionSettingsDTO settings);

        public abstract Task Enqueue(Message message);

        public abstract ValueTask DisposeAsync();
    }
}
