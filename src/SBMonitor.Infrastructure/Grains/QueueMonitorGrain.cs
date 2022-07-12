using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;

namespace SBMonitor.Infrastructure.Grains
{
    public class QueueMonitorGrain : MonitorGrain, IQueueMonitorGrain
    {
        protected override ServiceBusProcessor CreateProcessor()
        {
            return client.CreateProcessor(connectionProps.QueueName, options);
        }

        public QueueMonitorGrain(ILogger<QueueMonitorGrain> logger) : base()
        {
            _logger = logger;
        }
    }
}
