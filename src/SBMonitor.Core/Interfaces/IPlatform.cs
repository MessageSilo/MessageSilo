using SBMonitor.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Interfaces
{
    public interface IPlatform
    {
        string Id { get; }

        string DisplayName { get; }

        List<BusType> BusTypes { get; }
    }
}
