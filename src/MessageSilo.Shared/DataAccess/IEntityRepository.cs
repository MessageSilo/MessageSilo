using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.DataAccess
{
    public interface IEntityRepository
    {
        Task Add(IEnumerable<Entity> entities);

        Task Delete(string userId, IEnumerable<string> name);

        Task<IEnumerable<Entity>> Query(EntityKind kind, string? userId = null);

        Task<int> Count(string? userId = null);
    }
}
