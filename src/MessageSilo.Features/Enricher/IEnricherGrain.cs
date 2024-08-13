using MessageSilo.Domain.Entities;
using MessageSilo.Shared.Models;
using Orleans.Concurrency;

namespace MessageSilo.Features.Enricher
{
    public interface IEnricherGrain : IGrainWithStringKey
    {
        Task<Message?> Enrich(Message message);

        [OneWay]
        Task Init(EnricherDTO? dto = null);
    }
}
