using MessageSilo.Shared.Models;
using Orleans;

namespace MessageSilo.Features.Connection
{
    public interface IConnectionGrain : IMessageSenderGrain
    {
        Task Update(ConnectionSettingsDTO s);

        Task Delete();

        Task<ConnectionState> GetState();

        Task TransformAndSend(Message message);
    }
}
