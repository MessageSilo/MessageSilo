using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace MessageSilo.Shared.Serialization
{
    public static class YamlConverter
    {
        private static ISerializer serializer = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        private static IDeserializer deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public static string Serialize<T>(T input)
        {
            return serializer.Serialize(input);
        }

        public static T Deserialize<T>(string input)
        {
            return deserializer.Deserialize<T>(input);
        }
    }
}
