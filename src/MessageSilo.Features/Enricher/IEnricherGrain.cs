using MessageSilo.Shared.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Features.Enricher
{
    public interface IEnricherGrain : IEntityGrain<EnricherDTO, EnricherDTO>
    {
        Task<Message> Enrich(Message message);
    }
}
