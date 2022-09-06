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
        public static IMonitorGrain CreateMonitorGrain(this IGrainFactory grainFactory, BusType typeOfBus, Guid id)
        {
            switch (typeOfBus)
            {
                case BusType.Azure_Queue:
                    return grainFactory.GetGrain<IAzureQueueMonitorGrain>(id);
                case BusType.Azure_Topic:
                default:
                    return grainFactory.GetGrain<IAzureTopicMonitorGrain>(id);
            }
        }
    }
}
