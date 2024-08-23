using MessageSilo.Infrastructure.Interfaces;
using MessageSilo.Infrastructure.Services;
using static System.Environment;

namespace MessageSilo.SiloCTL
{
    public class CTLConfig
    {
        public const string API_VERSION = "v1";

        private const string CONFIG_FILE_NAME = "message-silo-config.yaml";

        public string LatestVersionInfoUrl { get; private set; } = "https://api.github.com/repos/MessageSilo/MessageSilo/releases/latest";

        public string Id { get; set; }

        public string ApiUrl { get; set; } = $"http://localhost:5000/api/{API_VERSION}/";

        private ConfigReader configReader;

        private readonly IYamlConverterService yamlConverterService = new YamlConverterService();

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
                Id = "local-user";
                var yaml = yamlConverterService.Serialize(this);
                File.WriteAllText(configPath, yaml);
            }
        }

        public void Load()
        {
            var appDataFolder = getAppDataFolder();
            var configPath = $"{appDataFolder}/{CONFIG_FILE_NAME}";
            configReader = new ConfigReader(configPath);

            var existing = yamlConverterService.Deserialize<CTLConfig>(configReader.FileContents.First());
            Id = existing.Id;
            ApiUrl = existing.ApiUrl;
        }

        public void Save()
        {
            var appDataFolder = getAppDataFolder();
            var configPath = $"{appDataFolder}/{CONFIG_FILE_NAME}";
            var yaml = yamlConverterService.Serialize(this);
            File.WriteAllText(configPath, yaml);
        }

        public override string ToString()
        {
            return yamlConverterService.Serialize(this);
        }

        private string getAppDataFolder() => Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "siloctl");
    }
}
