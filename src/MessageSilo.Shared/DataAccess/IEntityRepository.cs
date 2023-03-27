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

        Task Delete(string token, IEnumerable<string> name);

        Task<IEnumerable<Entity>> Query(EntityKind kind, string? token = null);
    }
}
