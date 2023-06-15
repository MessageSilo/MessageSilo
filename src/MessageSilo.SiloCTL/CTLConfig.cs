using MessageSilo.Shared.Serialization;
using Microsoft.Extensions.Configuration;
using System.Text;
using static System.Environment;

namespace MessageSilo.SiloCTL
{
    public class CTLConfig
    {
        private const string CONFIG_FILE_NAME = "message-silo-config.yaml";

        public string Auth0Domain { get; private set; } = "message-silo.eu.auth0.com";

        public string Auth0ClinetID { get; private set; } = "NDhyYxtmtQp51kpc22JMB3m46TymaZAp";

        public string Auth0RedirectUrl { get; private set; } = "http://localhost:4242";

        public string Auth0Audiance { get; private set; } = "https://api.message-silo.dev";

        public string LatestVersionInfoUrl { get; private set; } = "https://api.github.com/repos/MessageSilo/MessageSilo/releases/latest";

        public string Id { get; set; }

        public string Token { get; set; }

#if DEBUG
        public string ApiUrl { get; private set; } = "https://localhost:5000/api/v1";
#else
        public string ApiUrl { get; private set; } = "https://api.message-silo.dev/api/v1";
#endif

        private ConfigReader configReader;

        public CTLConfig()
        {
        }

        public void CreateIfNotExist()
        {
            var appDataFolder = getAppDataFolder();

            if (!Directory.Exists(appDataFolder))
                Directory.CreateDirectory(appDataFolder);

            var configPath = $"{appDataFolder}/{CONFIG_FILE_NAME}";

            if (!File.Exists(configPath))
            {
                Id = $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}";
                var yaml = YamlConverter.Serialize(this);
                File.WriteAllText(configPath, yaml);
            }
        }

        public void Load()
        {
            var appDataFolder = getAppDataFolder();
            var configPath = $"{appDataFolder}/{CONFIG_FILE_NAME}";
            configReader = new ConfigReader(configPath);

            var existing = YamlConverter.Deserialize<CTLConfig>(configReader.FileContents.First());
            Token = existing.Token;
        }

        public void Save()
        {
            var appDataFolder = getAppDataFolder();
            var configPath = $"{appDataFolder}/{CONFIG_FILE_NAME}";
            var yaml = YamlConverter.Serialize(this);
            File.WriteAllText(configPath, yaml);
        }

        public void Delete()
        {
            var appDataFolder = getAppDataFolder();

            var configPath = $"{appDataFolder}/{CONFIG_FILE_NAME}";

            if (File.Exists(configPath))
                File.Delete(configPath);
        }

        public override string ToString()
        {
            return YamlConverter.Serialize(this);
        }

        private string getAppDataFolder() => Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "siloctl");
    }
}
