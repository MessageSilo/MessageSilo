using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Shared
{
    public class Message
    {
        public string Id { get; set; }

        public DateTimeOffset EnqueuedTime { get; set; }

        public string Body { get; set; }

        public Message(string id, DateTimeOffset enqueuedTime, string body)
        {
            Id = id;
            EnqueuedTime = enqueuedTime;
            Body = body;
        }
    }
}
