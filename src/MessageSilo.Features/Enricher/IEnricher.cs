using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Features.Enricher
{
    public interface IEnricher
    {
        Task<string> TransformMessage(string message);
    }
}
