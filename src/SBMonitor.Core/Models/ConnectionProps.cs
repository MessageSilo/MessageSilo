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

        public MessagePlatformType TypeOfBus { get; set; } = MessagePlatformType.Azure_Queue;

        public string QueueName { get; set; } = string.Empty;

        public string TopicName { get; set; } = string.Empty;

        public string SubscriptionName { get; set; } = string.Empty;

        public ConnectionProps(string queueName)
        {
            QueueName = queueName;
            TypeOfBus = MessagePlatformType.Azure_Queue;
        }

        public ConnectionProps(string topicName, string subscriptionName)
        {
            TopicName = topicName;
            SubscriptionName = subscriptionName;
            TypeOfBus = MessagePlatformType.Azure_Topic;
        }

        public ConnectionProps()
        { }

        public void Update(ConnectionProps conn)
        {
            this.Name = conn.Name;
        }
    }
}
