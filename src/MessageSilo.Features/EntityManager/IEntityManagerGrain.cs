using FluentValidation.Results;
using MessageSilo.Shared.Models;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Features.EntityManager
{
    public interface IEntityManagerGrain : IGrainWithStringKey
    {
        Task<IEnumerable<Entity>> GetAll();

        Task<List<ValidationFailure>?> Upsert(Entity entity);

        Task<List<ValidationFailure>?> Delete(string entityName);
    }
}
