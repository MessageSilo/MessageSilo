using MessageSilo.Shared.Models;

namespace MessageSilo.Features.Enricher
{
    public interface IEnricherGrain : IGrainWithStringKey
    {
        Task<Message?> Enrich(Message message);
    }
}
