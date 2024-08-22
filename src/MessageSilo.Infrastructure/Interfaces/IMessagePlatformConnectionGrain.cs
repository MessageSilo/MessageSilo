using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;
using Orleans.Concurrency;

namespace MessageSilo.Infrastructure.Interfaces
{
    public interface IMessagePlatformConnectionGrain : IAsyncDisposable, IGrainWithStringKey
    {
        [OneWay]
        Task Init(ConnectionSettingsDTO settings);

        Task Enqueue(Message message);
    }
}
