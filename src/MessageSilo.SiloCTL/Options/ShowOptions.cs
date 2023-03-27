using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Util;
using ConsoleTables;
using MessageSilo.Shared.Enums;

namespace MessageSilo.SiloCTL.Options
{
    [Verb("show", HelpText = "Display one or many entities.\r\n\r\nPrints a table of the most important information about the specified entities.")]
    public class ShowOptions
    {
        private readonly ConsoleTable showEntitesTable = new ConsoleTable(new ConsoleTableOptions()
        {
            Columns = new[] { "KIND", "NAME", "TYPE", "STATUS" },
            EnableCount = true,
            NumberAlignment = Alignment.Right
        });

        [Option('n', "name", Required = false, HelpText = "Display detailed informations about a specific entity.")]
        public string Name { get; set; }

        public void Show(string token, MessageSiloAPIService api)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var conn = api.GetConnection(token, Name);

                if (conn is not null)
                    Console.WriteLine(conn);
                else
                    Console.WriteLine($"Connection '{Name}' not found.");

                return;
            }

            var existingConnections = api.GetConnections(token);
            var existingTargets = api.GetTargets(token);

            if (existingConnections.Count() + existingTargets.Count() == 0)
            {
                Console.WriteLine("No entities found.");
                return;
            }

            foreach (var c in existingConnections)
                showEntitesTable.AddRow(c.ConnectionSettings.Kind, c.ConnectionSettings.RowKey, c.ConnectionSettings.Type, c.Status);

            foreach (var t in existingTargets)
                showEntitesTable.AddRow(t.Kind, t.RowKey, t.Type, Status.Created);

            showEntitesTable.Write(Format.Default);
        }
    }
}
