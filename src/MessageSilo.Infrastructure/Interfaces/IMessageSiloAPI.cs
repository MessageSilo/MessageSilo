using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;

namespace MessageSilo.Infrastructure.Interfaces
{
    public interface IMessageSiloAPI
    {
        Task<IEnumerable<Entity>> List();

        Task Clear();

        Task<IEnumerable<EntityValidationErrors>?> Apply(ApplyDTO dto);
    }
}
