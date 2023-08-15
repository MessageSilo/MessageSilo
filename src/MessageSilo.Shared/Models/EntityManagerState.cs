using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.Models
{
    public class EntityManagerState
    {
        public Dictionary<int, double> Throughput { get; set; } = new Dictionary<int, double>();
        public List<Entity> Entities { get; set; } = new();
    }
}
