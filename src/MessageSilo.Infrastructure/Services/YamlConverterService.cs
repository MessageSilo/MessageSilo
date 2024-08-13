using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using MessageSilo.Infrastructure.Interfaces;

namespace MessageSilo.Infrastructure.Services
{
    public class YamlConverterService : IYamlConverterService
    {
        private readonly ISerializer serializer = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        private readonly IDeserializer deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public string Serialize<T>(T input)
        {
            return serializer.Serialize(input);
        }

        public T Deserialize<T>(string input)
        {
            return deserializer.Deserialize<T>(input);
        }
    }
}
