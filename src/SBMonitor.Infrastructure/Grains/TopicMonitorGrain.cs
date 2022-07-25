using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;

namespace SBMonitor.Infrastructure.Grains
{
    public class TopicMonitorGrain : MonitorGrain, ITopicMonitorGrain
    {
        protected override ServiceBusReceiver CreateReceiver()
        {
            return client.CreateReceiver(connectionProps.TopicName, connectionProps.SubscriptionName);
        }

        public TopicMonitorGrain(ILogger<TopicMonitorGrain> logger) : base()
        {
            base.logger = logger;
        }
    }
}
