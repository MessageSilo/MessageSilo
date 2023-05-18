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
using Microsoft.Azure.Amqp.Framing;

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
            var entitiesToDelete = api.GetEntities().Data!;

            entitiesToDelete = entitiesToDelete.Where(p => string.IsNullOrEmpty(Name) || p.RowKey == Name);

            foreach (var connection in entitiesToDelete.Where(p => p.Kind == EntityKind.Connection))
            {
                var result = api.Delete<ConnectionSettingsDTO>("Connections", connection.RowKey);
                displayResult(connection, result);
            }

            foreach (var target in entitiesToDelete.Where(p => p.Kind == EntityKind.Target))
            {
                var result = api.Delete<TargetDTO>("Targets", target.RowKey);
                displayResult(target, result);
            }

            foreach (var enricher in entitiesToDelete.Where(p => p.Kind == EntityKind.Enricher))
            {
                var result = api.Delete<EnricherDTO>("Enrichers", enricher.RowKey);
                displayResult(enricher, result);
            }
        }

        private void displayResult<R>(Entity entity, ApiContract<R> apiContract) where R : class
        {
            if (apiContract.Errors.Count == 0)
            {
                Console.WriteLine($"Entity '{entity.RowKey} - {entity.Kind}' deleted successfully!");
                return;
            }

            Console.WriteLine($"Cannot delete '{entity.RowKey}' because the following errors:");

            if (apiContract.Errors.Count > 0)
            {
                foreach (var err in apiContract.Errors)
                {
                    Console.WriteLine($"\t- {err.ErrorMessage}");
                }
            }
        }
    }
}
