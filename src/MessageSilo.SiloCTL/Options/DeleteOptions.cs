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
                api.DeleteConnection(Name);
                return;
            }

            var connections = api.GetConnections();

            foreach (var connection in connections)
            {
                api.DeleteConnection(connection.ConnectionSettings.RowKey);
            }

            var targets = api.GetTargets();

            foreach (var target in targets)
            {
                api.DeleteTarget(target.RowKey);
            }

            var enrichers = api.GetEnrichers();

            foreach (var enricher in enrichers)
            {
                api.DeleteTarget(enricher.RowKey);
            }
        }
    }
}
