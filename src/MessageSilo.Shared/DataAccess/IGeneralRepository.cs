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
        Task AddConnections(IEnumerable<string> connectionIds);

        Task DeleteConnections(IEnumerable<string> connectionIds);

        Task<IEnumerable<string>> QueryConnections(string token);
    }
}
