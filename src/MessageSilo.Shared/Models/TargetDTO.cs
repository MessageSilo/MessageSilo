using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Serialization;

namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class TargetDTO : Entity
    {
        //Common
        [Id(0)]
        public TargetType? Type { get; set; }

        //API
        [Id(1)]
        public string Url { get; set; }

        [Id(4)]
        public RetrySettings? Retry { get; set; }

        //Azure_EventGrid
        [Id(2)]
        public string Endpoint { get; set; }

        [Id(3)]
        public string AccessKey { get; set; }

        public TargetDTO()
        {
            Kind = EntityKind.Target;
        }

        public override string ToString()
        {
            return YamlConverter.Serialize(this);
        }
    }
}
