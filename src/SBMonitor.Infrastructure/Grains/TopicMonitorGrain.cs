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
            return _client.CreateProcessor(ConnectionProps.State.TopicName, ConnectionProps.State.SubscriptionName, _options);
        }

        public TopicMonitorGrain(ILogger<TopicMonitorGrain> logger, [PersistentState("topicMonitorGrainState")] IPersistentState<ConnectionProps> connectionProps) : base()
        {
            _logger = logger;
            ConnectionProps = connectionProps;
        }
    }
}
