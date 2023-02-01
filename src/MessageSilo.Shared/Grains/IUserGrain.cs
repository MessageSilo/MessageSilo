using MessageSilo.Shared.Models;
using Orleans;

namespace MessageSilo.Shared.Grains
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task AddConnection(ConnectionSettingsDTO setting);

        Task InitConnections();

        Task<List<ConnectionSettingsDTO>> GetConnections();

        Task DeleteConnection(Guid id);
    }
}
