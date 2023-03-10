using System;
using CommandLine;
using MessageSilo.Shared.Models;
using MessageSilo.SiloCTL;
using RestSharp;

namespace QuickStart
{
    class Program
    {
        public class Options
        {
            [Option('f', "filename", Required = true, HelpText = "Filename or directory to files to use to create the connections")]
            public string FileName { get; set; }
        }
        static void Main(string[] args)
        {
            var ctlConfig = new CTLConfig();
            ctlConfig.Init();

            var api = new MessageSiloAPIService(new RestClient("https://localhost:5000/api/v1"));

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       var connectionSettings = new List<ConnectionSettingsDTO>();
                       var configParser = new ConfigParser();
                       var configReader = new ConfigReader(o.FileName);

                       foreach (var config in configReader.FileContents)
                       {
                           var parsed = configParser.ConvertFromYAML<ConnectionSettingsDTO>(config);
                           parsed.Token = ctlConfig.Token;
                           connectionSettings.Add(parsed);
                       }

                       var existingConnections = api.GetConnections(ctlConfig.Token);

                       //Cleanup
                       var deletableConnIds = existingConnections.Where(ec => !connectionSettings.Any(p => ec == p.Id));

                       foreach (var deletableConnId in deletableConnIds)
                       {
                           api.DeleteConnection(deletableConnId);
                       }

                       //Init connections
                       foreach (var conn in connectionSettings)
                       {
                           api.UpdateConnection(conn);
                       };
                   });
        }
    }
}