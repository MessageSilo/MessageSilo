using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.Models
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public Message Message { get; private set; }

        public MessageReceivedEventArgs(Message message)
        {
            Message = message;
        }
    }
}
