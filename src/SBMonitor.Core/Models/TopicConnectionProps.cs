using SBMonitor.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Models
{
    public class TopicConnectionProps : ConnectionProps
    {
        public override BusType TypeOfBus { get; } = BusType.Topic;

        public string TopicName { get; set; } = string.Empty;

        public string SubscriptionName { get; set; } = string.Empty;
    }
}
