using SBMonitor.Core.Enums;
using SBMonitor.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Platforms
{
    public class AWS : IPlatform
    {
        public string Id => "aws";

        public string DisplayName => "Amazon Simple Queue Service";

        public List<MessagePlatformType> Types => new List<MessagePlatformType>() { MessagePlatformType.AWS_SQS };
    }
}
