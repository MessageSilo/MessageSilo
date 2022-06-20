using Orleans;
using SBMonitor.Core.Models;

namespace SBMonitor.Infrastructure.Grains.Interfaces
{
    public interface IConnectionManagerGrain : IGrainWithGuidKey
    {
        IList<IMonitorGrain<ConnectionProps>> List();
    }
}
