using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using SBMonitor.Core.Models;

namespace SBMonitor.Infrastructure.Grains
{
    public class TopicMonitorGrain : MonitorGrain<TopicConnectionProps>
    {
        protected override ServiceBusProcessor CreateProcessor()
        {
            return _client.CreateProcessor(ConnectionProps.State.TopicName, ConnectionProps.State.SubscriptionName, _options);
        }

        public TopicMonitorGrain(ILogger<TopicMonitorGrain> logger, [PersistentState("topicMonitorGrainState")] IPersistentState<TopicConnectionProps> connectionProps) : base()
        {
            _logger = logger;
            ConnectionProps = connectionProps;
        }
    }
}
