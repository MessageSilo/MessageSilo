using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.Models
{
    public interface IMessageSenderGrain : IGrainWithStringKey
    {
        Task Send(Message message);
    }
}
