using MessageSilo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.DataAccess
{
    public interface IMessageRepository<T> where T : Message
    {
        void Add(string connectionId, T message);
    }
}
