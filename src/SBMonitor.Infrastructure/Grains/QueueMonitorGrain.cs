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
        protected override ServiceBusReceiver CreateReceiver()
        {
            return client.CreateReceiver(connectionProps.QueueName);
        }

        public QueueMonitorGrain(ILogger<QueueMonitorGrain> logger) : base()
        {
            base.logger = logger;
        }
    }
}
