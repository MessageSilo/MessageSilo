using MessageSilo.Shared.Serialization;
using System.Text;

namespace MessageSilo.SiloCTL
{
    internal class CTLConfig
    {
        private const string CONFIG_FILE_PATH = "message-silo-config.yaml";

        public string Token { get; private set; }

#if DEBUG
        public string ApiUrl { get; private set; } = "https://localhost:5000/api/v1";
#else
        public string ApiUrl { get; private set; } = "https://api.message-silo.dev/api/v1";
#endif
        private ConfigReader configReader;

        public CTLConfig()
        {
        }

        public void Init()
        {
            if (!File.Exists(CONFIG_FILE_PATH))
            {
                Token = $"{Guid.NewGuid()}-{Guid.NewGuid()}";
                var yaml = YamlConverter.Serialize(this);
                File.WriteAllText(CONFIG_FILE_PATH, yaml);
            }

            configReader = new ConfigReader(CONFIG_FILE_PATH);

            var existing = YamlConverter.Deserialize<CTLConfig>(configReader.FileContents.First());
            Token = existing.Token;
            ApiUrl = existing.ApiUrl;
        }

        public override string ToString()
        {
            return YamlConverter.Serialize(this);
        }
    }
}
