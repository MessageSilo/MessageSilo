using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Orleans;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;

namespace SBMonitor.Infrastructure.Grains
{
    public class QueueMonitorGrain : MonitorGrain<QueueConnectionProps>
    {
        protected override ServiceBusProcessor CreateProcessor()
        {
            return _client.CreateProcessor(ConnectionProps.QueueName, _options);
        }

        public QueueMonitorGrain(ILogger<QueueMonitorGrain> logger) : base()
        {
            _logger = logger;
        }
    }
}
