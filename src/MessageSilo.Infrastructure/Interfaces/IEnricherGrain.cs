using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;
using Orleans.Concurrency;

namespace MessageSilo.Infrastructure.Interfaces
{
    public interface IEnricherGrain : IGrainWithStringKey
    {
        Task<Message?> Enrich(Message message);

        [OneWay]
        Task Init(EnricherDTO? dto = null);
    }
}
