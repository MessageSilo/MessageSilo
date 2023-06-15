using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.Models
{
    public class LastMessage
    {
        public Message Input { get; private set; }

        public Message? Output { get; private set; }

        public string? Error { get; private set; }

        public LastMessage(Message input)
        {
            Input = input.GetCopy();
        }

        public void SetOutput(Message? output, string? error)
        {
            Output = output?.GetCopy();
            Error = error;
        }
    }
}
