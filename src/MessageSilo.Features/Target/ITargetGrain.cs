using MessageSilo.Shared.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Features.Target
{
    public interface ITargetGrain : IMessageSenderGrain
    {
        Task Update(TargetDTO t);

        Task<TargetDTO> GetState();
    }
}
