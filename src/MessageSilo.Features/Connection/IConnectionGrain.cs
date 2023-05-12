using MessageSilo.Shared.Models;
using Orleans;

namespace MessageSilo.Features.Connection
{
    public interface IConnectionGrain : IEntityGrain<ConnectionSettingsDTO, ConnectionState>, IMessageSenderGrain
    {
        Task TransformAndSend(Message message);
    }
}
