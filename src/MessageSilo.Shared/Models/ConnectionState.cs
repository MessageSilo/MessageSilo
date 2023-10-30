using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Serialization;

namespace MessageSilo.Shared.Models
{
    public class ConnectionState
    {
        public ConnectionSettingsDTO ConnectionSettings { get; set; }

        public Status Status { get; set; }

        public string? InitializationError { get; set; }

        public override string ToString()
        {
            return YamlConverter.Serialize(this);
        }
    }
}
