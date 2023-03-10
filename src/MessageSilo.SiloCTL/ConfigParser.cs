using MessageSilo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MessageSilo.SiloCTL
{
    internal class ConfigParser
    {
        private readonly IDeserializer deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public T ConvertFromYAML<T>(string yamlContent)
        {
            var result = deserializer.Deserialize<T>(yamlContent);
            return result;
        }
    }
}
