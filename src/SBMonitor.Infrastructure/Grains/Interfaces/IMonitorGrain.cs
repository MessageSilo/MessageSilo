using Orleans;
using SBMonitor.Core.Models;

namespace SBMonitor.Infrastructure.Grains.Interfaces
{
    public interface IMonitorGrain : IGrainWithGuidKey
    {
        Task<ConnectionProps> ConnectAsync(ConnectionProps props);
    }

    public interface IQueueMonitorGrain : IMonitorGrain
    {

    }

    public interface ITopicMonitorGrain : IMonitorGrain
    {

    }
}
