using System;
using CommandLine;
using ConsoleTables;
using MessageSilo.Shared.Models;
using MessageSilo.SiloCTL;
using RestSharp;

namespace QuickStart
{
    class Program
    {
        [Verb("apply", HelpText = "Apply a configuration to a connection by file name or stdin.\r\n\r\nThe connection name must be specified. This connection will be created if it doesn't exist yet.\r\nYAML formats are accepted.")]
        public class ApplyOptions
        {
            [Option('f', "filename", Required = true, HelpText = "Filename or directory to files to use to create the connections.")]
            public string FileName { get; set; }
        }

        [Verb("show", HelpText = "Display one or many connections.\r\n\r\nPrints a table of the most important information about the specified connections.")]
        public class ShowOptions
        {
            [Option('n', "name", Required = false, HelpText = "Display detailed informations about a specific connection.")]
            public string ConnectionName { get; set; }
        }

        [Verb("delete", HelpText = "Delete one or many connections.")]
        public class DeleteOptions
        {
            [Option('n', "name", Required = false, HelpText = "Delete the specific connection.")]
            public string ConnectionName { get; set; }
        }

        static void Main(string[] args)
        {
            var ctlConfig = new CTLConfig();
            ctlConfig.Init();

            var api = new MessageSiloAPIService(new RestClient("https://localhost:5000/api/v1"));

            var existingConnections = api.GetConnections(ctlConfig.Token);

            Parser.Default.ParseArguments<ShowOptions, ApplyOptions, DeleteOptions>(args)
                   .WithParsed<ShowOptions>(o =>
                   {
                       if (!string.IsNullOrEmpty(o.ConnectionName))
                       {
                           var conn = api.GetConnection(ctlConfig.Token, o.ConnectionName);

                           if (conn is not null)
                               Console.WriteLine(conn);
                           else
                               Console.WriteLine($"Connection '{o.ConnectionName}' not found.");

                           return;
                       }

                       if (existingConnections.Count() == 0)
                       {
                           Console.WriteLine("No connections found.");
                           return;
                       }

                       foreach (var c in existingConnections)
                           Tables.ShowConnectionsTable.AddRow(c.ConnectionSettings.Name, c.ConnectionSettings.Type, c.Status);

                       Tables.ShowConnectionsTable.Write(Format.Default);
                   })
                   .WithParsed<ApplyOptions>(o =>
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

                       //Cleanup
                       foreach (var deletableConn in existingConnections)
                       {
                           api.DeleteConnection(deletableConn.ConnectionSettings.Token, deletableConn.ConnectionSettings.Name);
                       }

                       //Init connections
                       foreach (var conn in connectionSettings)
                       {
                           api.UpdateConnection(conn);
                       };
                   })
                   .WithParsed<DeleteOptions>(o =>
                   {
                       if (!string.IsNullOrEmpty(o.ConnectionName))
                       {
                           api.DeleteConnection(ctlConfig.Token, o.ConnectionName);
                       }
                   });
        }
    }
}