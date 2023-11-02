using MessageSilo.Shared.Models;
using Orleans;
using Orleans.Concurrency;

namespace MessageSilo.Features.Connection
{
    public interface IConnectionGrain : IEntityGrain<ConnectionSettingsDTO, ConnectionState>, IMessageSenderGrain
    {
        [OneWay]
        Task TransformAndSend(Message message);
    }
}
