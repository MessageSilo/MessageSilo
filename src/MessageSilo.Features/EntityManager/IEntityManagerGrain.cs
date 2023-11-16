using FluentValidation.Results;
using MessageSilo.Shared.Models;

namespace MessageSilo.Features.EntityManager
{
    public interface IEntityManagerGrain : IGrainWithStringKey
    {
        Task<IEnumerable<Entity>> List();

        Task<List<ValidationFailure>?> Upsert(Entity entity);

        Task<ConnectionSettingsDTO> GetConnectionSettings(string name);

        Task<TargetDTO> GetTargetSettings(string name);

        Task<EnricherDTO> GetEnricherSettings(string name);

        Task<int> GetScale();

        Task Clear();

        Task<List<EntityValidationErrors>> Vaidate(ApplyDTO dto);

        Task Apply(ApplyDTO dto);
    }
}
