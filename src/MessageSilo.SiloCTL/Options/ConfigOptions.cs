using CommandLine;

namespace MessageSilo.SiloCTL.Options
{
    [Verb("config", HelpText = "Display the current config and context.")]
    public class ConfigOptions : Options
    {
        [Option(longName: "set-api", Required = false, HelpText = "Set the API url, what siloctl is using.")]
        public string Url { get; set; }

        public ConfigOptions() : base()
        {
        }

        public void Show()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                config.ApiUrl = $"{Url}/api/{CTLConfig.API_VERSION}/";
                config.Save();

                return;
            }

            Console.WriteLine(config);
        }


    }
}
