using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Serialization;
using RestSharp;

namespace MessageSilo.Shared.Models
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

        //AI
        public string ApiKey { get; set; }

        public string Command { get; set; }

        public EnricherDTO()
        {
            Kind = EntityKind.Enricher;
        }

        public override string ToString()
        {
            return YamlConverter.Serialize(this);
        }

        public EnricherDTO GetCopy()
        {
            var yaml = ToString();
            return YamlConverter.Deserialize<EnricherDTO>(yaml);
        }
    }
}
