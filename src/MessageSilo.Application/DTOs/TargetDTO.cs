using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;

namespace MessageSilo.Application.DTOs
{
    public class TargetDTO : Entity
    {
        //Common
        public TargetType? Type { get; set; }

        //API
        public string Url { get; set; }

        public RetrySettings? Retry { get; set; }

        //Azure_EventGrid
        public string Endpoint { get; set; }

        public string AccessKey { get; set; }

        //AI_Router
        public List<AIRouterRule> Rules { get; set; } = [];

        public string ApiKey { get; set; }

        public string Model { get; set; }

        public TargetDTO()
        {
            Kind = EntityKind.Target;
        }
    }
}
