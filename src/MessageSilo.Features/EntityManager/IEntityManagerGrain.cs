using FluentValidation.Results;
using MessageSilo.Shared.Models;

namespace MessageSilo.Features.EntityManager
{
    public interface IEntityManagerGrain : IGrainWithStringKey
    {
        Task<IEnumerable<Entity>> GetAll();

        Task<List<ValidationFailure>?> Upsert(Entity entity);

        Task<List<ValidationFailure>?> Delete(string entityName);
    }
}
