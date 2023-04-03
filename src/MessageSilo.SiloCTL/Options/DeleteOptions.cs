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
    public class DeleteOptions
    {
        [Option('n', "name", Required = false, HelpText = "Name of the entity to delete.")]
        public string Name { get; set; }

        public void Delete(string token, MessageSiloAPIService api)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                api.DeleteConnection(token, Name);
                return;
            }

            var connections = api.GetConnections(token);

            foreach (var connection in connections)
            {
                api.DeleteConnection(token, connection.ConnectionSettings.RowKey);
            }

            var targets = api.GetTargets(token);

            foreach (var target in targets)
            {
                api.DeleteTarget(token, target.RowKey);
            }
        }
    }
}
