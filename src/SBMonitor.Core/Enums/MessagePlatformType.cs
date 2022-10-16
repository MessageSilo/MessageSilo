using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Enums
{
    public enum MessagePlatformType
    {
        [Description("Queue")]
        Azure_Queue,

        [Description("Topic")]
        Azure_Topic,

        [Description("SQS")]
        AWS_SQS
    }
}
