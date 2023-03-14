using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.Models
{
    public class ConnectionState
    {
        public ConnectionSettingsDTO ConnectionSettings { get; set; }

        public string Status { get; set; }

        public long? LastProcessedMessageSequenceNumber { get; set; }
    }
}
