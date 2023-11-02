using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Serialization;

namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class ConnectionState
    {
        [Id(0)]
        public ConnectionSettingsDTO ConnectionSettings { get; set; }

        [Id(1)]
        public Status Status { get; set; }

        [Id(2)]
        public string? InitializationError { get; set; }

        public override string ToString()
        {
            return YamlConverter.Serialize(this);
        }
    }
}
