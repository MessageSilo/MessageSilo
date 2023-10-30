using FluentValidation.Results;
using MessageSilo.Shared.Models;
using Orleans;
using Orleans.Concurrency;

namespace MessageSilo.Features.EntityManager
{
    public interface IEntityManagerGrain : IGrainWithStringKey
    {
        Task<IEnumerable<Entity>> GetAll();

        Task<List<ValidationFailure>?> Upsert(Entity entity);

        Task<List<ValidationFailure>?> Delete(string entityName);

        [OneWay]
        Task IncreaseUsedThroughput(string messageBody);

        Task<double> GetUsedThroughput(int date);
    }
}
