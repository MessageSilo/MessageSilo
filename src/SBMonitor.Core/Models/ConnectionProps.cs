using SBMonitor.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Models
{
    public abstract class ConnectionProps
    {
        public string Name { get; set; } = string.Empty;

        public string ConnectionString { get; set; } = string.Empty;

        public abstract BusType TypeOfBus { get; }
    }
}
