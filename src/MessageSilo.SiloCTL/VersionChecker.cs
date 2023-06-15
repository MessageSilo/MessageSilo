using RestSharp;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MessageSilo.SiloCTL
{
    public class VersionChecker
    {
        public void CheckLatestVersion(string latestVersionInfoUrl)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            var client = new RestClient(latestVersionInfoUrl);
            var request = new RestRequest("", Method.Get);
            var response = client.Execute(request);

            if (!response.IsSuccessStatusCode)
                throw response.ErrorException!;

            var data = JsonSerializer.Deserialize<JsonNode>(response.Content!);

            var latestVersion = data["tag_name"].GetValue<string>().TrimStart('v');

            if (currentVersion != latestVersion)
                Console.WriteLine($"New version available! Please update siloctl! Latest version: {latestVersion}");

            Console.ResetColor();
        }

        private string currentVersion => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }
}
