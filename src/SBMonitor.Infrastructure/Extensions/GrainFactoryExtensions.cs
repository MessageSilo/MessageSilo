using Orleans;
using SBMonitor.Core.Enums;
using SBMonitor.Infrastructure.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Infrastructure.Extensions
{
    public static class GrainFactoryExtensions
    {
        public static IMonitorGrain MonitorGrain(this IGrainFactory grainFactory, BusType typeOfBus, Guid id)
        {
            switch (typeOfBus)
            {
                case BusType.Queue:
                    return grainFactory.GetGrain<IQueueMonitorGrain>(id);
                case BusType.Topic:
                default:
                    return grainFactory.GetGrain<ITopicMonitorGrain>(id);
            }
        }
    }
}
