using CommandLine;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Serialization;

namespace MessageSilo.SiloCTL.Options
{
    [Verb("apply", HelpText = "Apply a configuration to an entity by file name or stdin.\r\n\r\nThe entity name must be specified. This entity will be created if it doesn't exist yet.\r\nYAML formats are accepted.")]
    public class ApplyOptions : AuthorizedOptions
    {
        public ApplyOptions() : base()
        {
        }

        [Option('f', "filename", Required = true, HelpText = "Filename or directory to files to use to create the entities.")]
        public string FileName { get; set; }

        public IEnumerable<ConnectionSettingsDTO> InitConnections(IEnumerable<TargetDTO> targets)
        {
            var connectionSettings = new List<ConnectionSettingsDTO>();
            var configReader = new ConfigReader(FileName);

            foreach (var config in configReader.FileContents.Where(p => p.Contains($"kind: {EntityKind.Connection}")))
            {
                var parsed = YamlConverter.Deserialize<ConnectionSettingsDTO>(config);
                parsed.TargetKind = targets.Any(p => p.Id == parsed.TargetId) ? EntityKind.Target : EntityKind.Connection;
                connectionSettings.Add(parsed);
            }

            foreach (var conn in connectionSettings.OrderBy(p => p.Target))
            {
                var result = api.Update<ConnectionSettingsDTO, ConnectionState>("Connections", conn);
                displayResult(conn, result);
            };

            return connectionSettings;
        }

        public IEnumerable<TargetDTO> InitTargets()
        {
            var targets = new List<TargetDTO>();
            var configReader = new ConfigReader(FileName);

            foreach (var config in configReader.FileContents.Where(p => p.Contains($"kind: {EntityKind.Target}")))
            {
                var parsed = YamlConverter.Deserialize<TargetDTO>(config);
                targets.Add(parsed);
            }

            foreach (var target in targets)
            {
                var result = api.Update<TargetDTO, TargetDTO>("Targets", target);
                displayResult(target, result);
            };

            return targets;
        }

        public IEnumerable<EnricherDTO> InitEnrichers()
        {
            var enrichers = new List<EnricherDTO>();
            var configReader = new ConfigReader(FileName);

            foreach (var config in configReader.FileContents.Where(p => p.Contains($"kind: {EntityKind.Enricher}")))
            {
                var parsed = YamlConverter.Deserialize<EnricherDTO>(config);
                enrichers.Add(parsed);
            }

            foreach (var enricher in enrichers)
            {
                var result = api.Update<EnricherDTO, EnricherDTO>("Enrichers", enricher);
                displayResult(enricher, result);
            };

            return enrichers;
        }

        private void displayResult<R>(Entity entity, ApiContract<R> apiContract) where R : class
        {
            if (apiContract.Errors.Count == 0)
            {
                Console.WriteLine($"Changes applied on '{entity.RowKey} - {entity.Kind}' successfully!");
                return;
            }

            Console.WriteLine($"Cannot apply changes on '{entity.RowKey}' because the following errors:");

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
