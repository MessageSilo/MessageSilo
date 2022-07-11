using Orleans;
using SBMonitor.Core.Models;

namespace SBMonitor.Infrastructure.Grains.Interfaces
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task AddMonitorGrain(Guid id);

        Task RemoveMonitorGrain(Guid id);
    }
}
