using SBMonitor.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Shared
{
    public class ConnectionSettingsDTO
    {
        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public MessagePlatformType Type { get; set; }

        public string QueueName { get; set; }

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }

        public string CorrectorFuncBody { get; set; }
    }
}
