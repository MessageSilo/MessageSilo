using CommandLine;
using ConsoleTables;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Serialization;
using MessageSilo.SiloCTL;
using MessageSilo.SiloCTL.Options;
using RestSharp;

namespace QuickStart
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctlConfig = new CTLConfig();
            ctlConfig.Init();

            var api = new MessageSiloAPIService(new RestClient(ctlConfig.ApiUrl));

            Parser.Default.ParseArguments<ShowOptions, ApplyOptions, ConfigOptions>(args)
                   .WithParsed<ShowOptions>(o =>
                   {
                       o.Show(ctlConfig.Token, api);
                   })
                   .WithParsed<ApplyOptions>(o =>
                   {
                       var targets = o.InitTargets(ctlConfig.Token, api);
                       o.InitConnections(ctlConfig.Token, api, targets);
                   })
                   .WithParsed<ConfigOptions>(o =>
                   {
                       Console.WriteLine(ctlConfig.ToString());
                   });
        }
    }
}