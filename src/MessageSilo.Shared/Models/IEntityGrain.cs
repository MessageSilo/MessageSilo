using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.Models
{
    public interface IEntityGrain<DTO, S> : IGrainWithStringKey where DTO : Entity where S : class
    {
        Task Update(DTO state, string secKey);

        Task Delete();

        Task<S> GetState();
    }
}
