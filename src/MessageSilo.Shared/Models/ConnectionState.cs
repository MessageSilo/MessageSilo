using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Serialization;
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

        public Status Status { get; set; }

        public override string ToString()
        {
            return YamlConverter.Serialize(this);
        }
    }
}
