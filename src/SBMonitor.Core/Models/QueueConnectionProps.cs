using SBMonitor.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Models
{
    public class QueueConnectionProps : ConnectionProps
    {
        public override BusType TypeOfBus { get; } = BusType.Queue;

        public string QueueName { get; set; } = string.Empty;
    }
}
