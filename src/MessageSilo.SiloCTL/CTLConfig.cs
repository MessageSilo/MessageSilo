using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace MessageSilo.SiloCTL
{
    internal class CTLConfig
    {
        private const string CONFIG_FILE_PATH = "message-silo-config.yaml";

        public string Token { get; private set; }

        private ConfigReader configReader;

        private ConfigParser configParser;

        private readonly ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public CTLConfig()
        {
        }

        public void Init()
        {
            if (!File.Exists(CONFIG_FILE_PATH))
            {
                Token = $"{Guid.NewGuid()}-{Guid.NewGuid()}";
                var yaml = serializer.Serialize(this);
                File.WriteAllText(CONFIG_FILE_PATH, yaml);
            }

            configParser = new ConfigParser();
            configReader = new ConfigReader(CONFIG_FILE_PATH);

            var existing = configParser.ConvertFromYAML<CTLConfig>(configReader.FileContents.First());
            Token = existing.Token;
        }
    }
}
