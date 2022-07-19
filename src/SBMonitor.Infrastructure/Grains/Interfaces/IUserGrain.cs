using Orleans;
using SBMonitor.Core.Models;

namespace SBMonitor.Infrastructure.Grains.Interfaces
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task AddOrUpdateConnection(ConnectionProps conn);

        Task RemoveConnection(Guid id);

        Task<IList<ConnectionProps>> Connections();
    }
}
