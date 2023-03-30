using MessageSilo.Shared.Serialization;
using Microsoft.Extensions.Configuration;
using System.Text;
using static System.Environment;

namespace MessageSilo.SiloCTL
{
    internal class CTLConfig
    {
        private const string CONFIG_FILE_NAME = "message-silo-config.yaml";

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
            var appDataFolder = Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "siloctl");

            if(!Directory.Exists(appDataFolder))
                Directory.CreateDirectory(appDataFolder);

            var configPath = $"{appDataFolder}/{CONFIG_FILE_NAME}";

            if (!File.Exists(configPath))
            {
                Token = $"{Guid.NewGuid()}-{Guid.NewGuid()}";
                var yaml = YamlConverter.Serialize(this);
                File.WriteAllText(configPath, yaml);
            }

            configReader = new ConfigReader(configPath);

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
