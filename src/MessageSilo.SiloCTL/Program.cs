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

                new VersionChecker().CheckLatestVersion(config.LatestVersionInfoUrl);

                if (interactiveMode)
                    args = Console.ReadLine()!.Split();

                Parser.Default.ParseArguments<ShowOptions, ApplyOptions, ConfigOptions, ClearOptions, LogoutOptions>(args)
                           .WithParsed<ShowOptions>(o =>
                           {
                               o.Show();
                           })
                           .WithParsed<ApplyOptions>(o =>
                           {
                               o.Apply();
                           })
                           .WithParsed<ConfigOptions>(o =>
                           {
                               o.Show();
                           })
                           .WithParsed<ClearOptions>(o =>
                           {
                               o.Clear();
                           })
                           .WithParsed<LogoutOptions>(o =>
                           {
                               o.Logout();
                           });
            } while (interactiveMode);
        }
    }
}