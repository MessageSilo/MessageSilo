using Orleans;
using SBMonitor.Core.Models;

namespace SBMonitor.Infrastructure.Grains.Interfaces
{
    public interface IMonitorGrain<T> : IGrainWithGuidKey where T : ConnectionProps
    {
        void Connect(T props);
    }
}
