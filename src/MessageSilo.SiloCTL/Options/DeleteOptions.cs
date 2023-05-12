using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Util;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Serialization;
using MessageSilo.Shared.Enums;
using ConsoleTables;

namespace MessageSilo.SiloCTL.Options
{
    [Verb("delete", HelpText = "Deletes one or all entities.")]
    public class DeleteOptions : AuthorizedOptions
    {
        public DeleteOptions() : base()
        {
        }

        [Option('n', "name", Required = false, HelpText = "Name of the entity to delete.")]
        public string Name { get; set; }

        public void Delete()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                api.Delete("Connections", Name);
                return;
            }

            var connections = api.Get<ConnectionState>("Connections");

            foreach (var connection in connections.Data)
            {
                api.Delete("Connections", connection.ConnectionSettings.RowKey);
            }

            var targets = api.Get<TargetDTO>("Targets");

            foreach (var target in targets.Data)
            {
                api.Delete("Targets", target.RowKey);
            }

            var enrichers = api.Get<EnricherDTO>("Enrichers");

            foreach (var enricher in enrichers.Data)
            {
                api.Delete("Enrichers", enricher.RowKey);
            }
        }
    }
}
