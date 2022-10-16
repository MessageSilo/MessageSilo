using SBMonitor.Core.Enums;
using SBMonitor.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Platforms
{
    public class Azure : IPlatform
    {
        public string Id => "azure";

        public string DisplayName => "Azure Service Bus";

        public List<MessagePlatformType> Types => new List<MessagePlatformType>() { MessagePlatformType.Azure_Queue, MessagePlatformType.Azure_Topic };
    }
}
