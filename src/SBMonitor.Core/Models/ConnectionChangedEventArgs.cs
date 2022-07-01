using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.Models
{
    public class ConnectionChangedEventArgs : EventArgs
    {
        public ConnectionProps ConnectionProps { get; set; }

        public ConnectionChangedEventArgs(ConnectionProps connectionProps) : base()
        {
            ConnectionProps = connectionProps;
        }
    }
}
