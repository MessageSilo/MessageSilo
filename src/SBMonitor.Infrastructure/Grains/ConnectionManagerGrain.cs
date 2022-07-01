using Orleans;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Infrastructure.Grains
{
    public class ConnectionManagerGrain : Grain, IConnectionManagerGrain
    {
        private IList<IMonitorGrain> _monitorGrains { get; set; } = new List<IMonitorGrain>();

        public void Add(IMonitorGrain grain)
        {
            _monitorGrains.Add(grain);
        }
    }
}
