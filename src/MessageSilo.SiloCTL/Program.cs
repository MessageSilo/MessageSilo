using CommandLine;
using MessageSilo.SiloCTL.Options;

namespace MessageSilo.SiloCTL
{
    class Program
    {
        static void Main(string[] args)
        {
            var interactiveMode = args.Any(p => p == "-it");

            do
            {
                var config = new CTLConfig();
                config.CreateIfNotExist();
                config.Load();

                //new VersionChecker().CheckLatestVersion(config.LatestVersionInfoUrl);

                if (interactiveMode)
                    args = Console.ReadLine()!.Split();

                var client = new HttpClient
                {
                    BaseAddress = new Uri(config.ApiUrl),
                };

                var api = new MessageSiloAPI(client);

                Parser.Default.ParseArguments<ShowOptions, ApplyOptions, ConfigOptions, ClearOptions>(args)
                           .WithParsed<ShowOptions>(o =>
                           {
                               o.Show(api);
                           })
                           .WithParsed<ApplyOptions>(o =>
                           {
                               o.Apply(api);
                           })
                           .WithParsed<ConfigOptions>(o =>
                           {
                               o.Show();
                           })
                           .WithParsed<ClearOptions>(o =>
                           {
                               o.Clear(api);
                           });
            } while (interactiveMode);
        }
    }
}