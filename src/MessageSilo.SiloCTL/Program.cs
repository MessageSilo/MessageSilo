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

                new VersionChecker().CheckLatestVersion(config.LatestVersionInfoUrl);

                if (interactiveMode)
                    args = Console.ReadLine()!.Split();

                Parser.Default.ParseArguments<ShowOptions, ApplyOptions, ConfigOptions, DeleteOptions, LogoutOptions>(args)
                           .WithParsed<ShowOptions>(o =>
                           {
                               o.Show();
                           })
                           .WithParsed<ApplyOptions>(o =>
                           {
                               var targets = o.InitTargets();
                               o.InitEnrichers();
                               o.InitConnections(targets);
                           })
                           .WithParsed<ConfigOptions>(o =>
                           {
                               o.Show();
                           })
                           .WithParsed<DeleteOptions>(o =>
                           {
                               o.Delete();
                           })
                           .WithParsed<LogoutOptions>(o =>
                           {
                               o.Logout();
                           });
            } while (interactiveMode);
        }
    }
}