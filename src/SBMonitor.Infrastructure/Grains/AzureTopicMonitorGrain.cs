using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;

namespace SBMonitor.Infrastructure.Grains
{
    public class AzureTopicMonitorGrain : MonitorGrain, IAzureTopicMonitorGrain
    {
        protected override ServiceBusReceiver CreateReceiver()
        {
            return client.CreateReceiver(connectionProps.TopicName, connectionProps.SubscriptionName);
        }

        public AzureTopicMonitorGrain(ILogger<AzureTopicMonitorGrain> logger) : base()
        {
            base.logger = logger;
        }
    }
}
