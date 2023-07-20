using MessageSilo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Features.MessageSiloApi
{
    public interface IMessageSiloAPI
    {
        ApiContract<IEnumerable<Entity>> GetEntities();

        ApiContract<IEnumerable<R>> Get<R>(string controller) where R : class;

        ApiContract<LastMessage> GetLastMessage(string controller, string name);

        ApiContract<R> Get<R>(string controller, string name) where R : class;

        ApiContract<R> Update<DTO, R>(string controller, DTO dto) where DTO : Entity where R : class;

        ApiContract<R> Delete<R>(string controller, string name) where R : class;
    }
}
