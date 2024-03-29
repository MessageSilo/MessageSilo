﻿using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Serialization;
using RestSharp;
using YamlDotNet.Serialization;

namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class EnricherDTO : Entity
    {
        //Common
        [Id(0)]
        public EnricherType? Type { get; set; }

        //Inline
        [Id(1)]
        public string Function { get; set; }

        //API
        [Id(2)]
        public string Url { get; set; }

        [Id(3)]
        public Method? Method { get; set; }

        [Id(6)]
        public RetrySettings? Retry { get; set; }

        //AI
        [Id(4)]
        public string ApiKey { get; set; }

        [Id(5)]
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
