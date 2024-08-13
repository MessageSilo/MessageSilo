using MessageSilo.Domain.Entities;
using Orleans.Concurrency;

namespace MessageSilo.Shared.Models
{
    public interface IMessagePlatformConnectionGrain : IAsyncDisposable, IGrainWithStringKey
    {
        [OneWay]
        Task Init(ConnectionSettingsDTO settings);

        Task Enqueue(Message message);
    }
}
