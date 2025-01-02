using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;
using RestSharp;

namespace MessageSilo.Application.DTOs
{
    public class EnricherDTO : Entity
    {
        //Common
        public EnricherType? Type { get; set; }

        //Inline
        public string Function { get; set; }

        //API
        public string Url { get; set; }

        public Method? Method { get; set; }

        public RetrySettings? Retry { get; set; }

        //AI
        public string ApiKey { get; set; }

        public string Model { get; set; }

        public string Command { get; set; }

        public EnricherDTO()
        {
            Kind = EntityKind.Enricher;
        }
    }
}
