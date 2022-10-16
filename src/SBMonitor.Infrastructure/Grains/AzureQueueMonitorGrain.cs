using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;

namespace SBMonitor.Infrastructure.Grains
{
    public class AzureQueueMonitorGrain : MonitorGrain, IAzureQueueMonitorGrain
    {
        protected override ServiceBusReceiver CreateReceiver()
        {
            return client.CreateReceiver(connectionProps.QueueName);
        }

        public AzureQueueMonitorGrain(ILogger<AzureQueueMonitorGrain> logger) : base()
        {
            base.logger = logger;
        }
    }
}
