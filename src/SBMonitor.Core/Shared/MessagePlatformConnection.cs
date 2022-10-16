using SBMonitor.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Shared
{
    public abstract class MessagePlatformConnection : IMessagePlatformConnection
    {
        public string Name { get; protected set; }

        public string ConnectionString { get; protected set; }

        public MessagePlatformType Type { get; protected set; }

        public event EventHandler DeadLetterMessageReceived;

        public abstract void StartProcessingDeadLetterMessages();

        protected Task onDeadLetterMessageReceived(EventArgs e)
        {
            DeadLetterMessageReceived?.Invoke(this, e);
            return Task.CompletedTask;
        }
    }
}
