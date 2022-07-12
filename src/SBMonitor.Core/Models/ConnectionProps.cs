using SBMonitor.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Models
{
    public class ConnectionProps
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string ConnectionString { get; set; } = string.Empty;

        public BusType TypeOfBus { get; set; } = BusType.Queue;

        public string QueueName { get; set; } = string.Empty;

        public string TopicName { get; set; } = string.Empty;

        public string SubscriptionName { get; set; } = string.Empty;

        public ConnectionProps(string queueName)
        {
            QueueName = queueName;
            TypeOfBus = BusType.Queue;
        }

        public ConnectionProps(string topicName, string subscriptionName)
        {
            TopicName = topicName;
            SubscriptionName = subscriptionName;
            TypeOfBus = BusType.Topic;
        }

        public ConnectionProps()
        { }
    }
}
