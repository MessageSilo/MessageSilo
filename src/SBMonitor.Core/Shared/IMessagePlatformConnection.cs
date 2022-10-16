using SBMonitor.Core.Enums;
using SBMonitor.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Shared
{
    public interface IMessagePlatformConnection
    {
        string Name { get; }

        string ConnectionString { get; }

        MessagePlatformType Type { get; }

        void StartProcessingDeadLetterMessages();

        event EventHandler DeadLetterMessageReceived;
    }
}
