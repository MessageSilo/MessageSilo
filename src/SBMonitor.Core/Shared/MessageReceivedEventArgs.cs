using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Shared
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string MessageBody { get; set; }
    }
}
