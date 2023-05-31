using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public EnricherDTO()
        {
            Kind = EntityKind.Enricher;
        }

        public override string ToString()
        {
            return YamlConverter.Serialize(this);
        }
    }
}
