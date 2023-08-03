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
using MessageSilo.Shared.Models;

namespace MessageSilo.SiloCTL.Options
{
    [Verb("show", HelpText = "Display one or many entities.\r\n\r\nPrints a table of the most important information about the specified entities.")]
    public class ShowOptions : AuthorizedOptions
    {
        private readonly ConsoleTable showEntitesTable = new ConsoleTable(new ConsoleTableOptions()
        {
            Columns = new[] { "KIND", "NAME", "TYPE", "STATUS" },
            EnableCount = true,
            NumberAlignment = Alignment.Right
        });

        public ShowOptions() : base()
        {
        }

        [Option('n', "name", Required = false, HelpText = "Display detailed information about a specific entity.")]
        public string Name { get; set; }

        public void Show()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var entities = api.GetEntities();

                var entity = entities.Data?.FirstOrDefault(p => p.RowKey == Name);

                if (entity is not null)
                {
                    switch (entity.Kind)
                    {
                        case EntityKind.Connection:
                            Console.WriteLine(api.Get<ConnectionState>("Connections", entity.RowKey).Data);
                            Console.WriteLine(api.GetLastMessage("Connections", entity.RowKey).Data);
                            break;
                        case EntityKind.Enricher:
                            Console.WriteLine(api.Get<EnricherDTO>("Enrichers", entity.RowKey).Data);
                            Console.WriteLine(api.GetLastMessage("Enrichers", entity.RowKey).Data);
                            break;
                        case EntityKind.Target:
                            Console.WriteLine(api.Get<TargetDTO>("Targets", entity.RowKey).Data);
                            Console.WriteLine(api.GetLastMessage("Targets", entity.RowKey).Data);
                            break;
                    }
                }
                else
                    Console.WriteLine($"Entity '{Name}' not found.");

                return;
            }

            var existingConnections = api.Get<ConnectionState>("Connections").Data!;
            var existingTargets = api.Get<TargetDTO>("Targets").Data!;
            var existingEnrichers = api.Get<EnricherDTO>("Enrichers").Data!;

            if ((existingConnections.Count() + existingTargets.Count() + existingEnrichers.Count()) == 0)
            {
                Console.WriteLine("No entities found.");
                return;
            }

            foreach (var c in existingConnections)
                showEntitesTable.AddRow(c.ConnectionSettings.Kind, c.ConnectionSettings.RowKey, c.ConnectionSettings.Type, c.Status);

            foreach (var t in existingTargets)
                showEntitesTable.AddRow(t.Kind, t.RowKey, t.Type, Status.Created);

            foreach (var t in existingEnrichers)
                showEntitesTable.AddRow(t.Kind, t.RowKey, t.Type, Status.Created);

            showEntitesTable.Write(Format.Default);
        }
    }
}
