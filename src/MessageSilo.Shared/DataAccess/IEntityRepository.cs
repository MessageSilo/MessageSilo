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
        Task Upsert(Entity entity);

        Task Delete(string userId, string entityName);

        Task<IEnumerable<Entity>> Query(EntityKind? kind = null, string? userId = null);
    }
}
