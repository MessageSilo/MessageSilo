using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Models
{
    public class User
    {
        public IList<ConnectionProps> Connections { get; set; } = new List<ConnectionProps>();
    }
}
