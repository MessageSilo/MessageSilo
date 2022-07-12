using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;

namespace SBMonitor.Infrastructure.Grains
{
    public class TopicMonitorGrain : MonitorGrain, ITopicMonitorGrain
    {
        protected override ServiceBusProcessor CreateProcessor()
        {
            return client.CreateProcessor(connectionProps.TopicName, connectionProps.SubscriptionName, options);
        }

        public TopicMonitorGrain(ILogger<TopicMonitorGrain> logger) : base()
        {
            _logger = logger;
        }
    }
}
