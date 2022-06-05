using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using SBMonitor.Core.Models;

namespace SBMonitor.Infrastructure.Grains
{
    public class TopicMonitorGrain : MonitorGrain<TopicConnectionProps>
    {
        protected override ServiceBusProcessor CreateProcessor()
        {
            return _client.CreateProcessor(ConnectionProps.TopicName, ConnectionProps.SubscriptionName, _options);
        }

        public TopicMonitorGrain(ILogger<TopicMonitorGrain> logger) : base()
        {
            _logger = logger;
        }
    }
}
