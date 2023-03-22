using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.DataAccess
{
    public interface IGeneralRepository
    {
        Task Add(EntityKind kind, IEnumerable<string> connectionIds);

        Task Delete(EntityKind kind, IEnumerable<string> connectionIds);

        Task<IEnumerable<string>> Query(EntityKind kind, string? token = null);
    }
}
