using CommandLine;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;

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

            entitiesToDelete = entitiesToDelete.Where(p => string.IsNullOrEmpty(Name) || p.Name == Name);

            foreach (var connection in entitiesToDelete.Where(p => p.Kind == EntityKind.Connection))
            {
                var result = api.Delete<ConnectionSettingsDTO>("Connections", connection.Name);
                displayResult(connection, result);
            }

            foreach (var target in entitiesToDelete.Where(p => p.Kind == EntityKind.Target))
            {
                var result = api.Delete<TargetDTO>("Targets", target.Name);
                displayResult(target, result);
            }

            foreach (var enricher in entitiesToDelete.Where(p => p.Kind == EntityKind.Enricher))
            {
                var result = api.Delete<EnricherDTO>("Enrichers", enricher.Name);
                displayResult(enricher, result);
            }
        }

        private void displayResult<R>(Entity entity, ApiContract<R> apiContract) where R : class
        {
            if (apiContract.Errors.Count == 0)
            {
                Console.WriteLine($"Entity '{entity.Name} - {entity.Kind}' deleted successfully!");
                return;
            }

            Console.WriteLine($"Cannot delete '{entity.Name}' because the following errors:");

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
