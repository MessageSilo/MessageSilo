using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;

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
    }
}
