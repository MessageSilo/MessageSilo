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
    [Verb("apply", HelpText = "Apply a configuration to an entity by file name or stdin.\r\n\r\nThe entity name must be specified. This entity will be created if it doesn't exist yet.\r\nYAML formats are accepted.")]
    public class ApplyOptions
    {
        [Option('f', "filename", Required = true, HelpText = "Filename or directory to files to use to create the entities.")]
        public string FileName { get; set; }

        public IEnumerable<ConnectionSettingsDTO> InitConnections(string token, MessageSiloAPIService api, IEnumerable<TargetDTO> targets)
        {
            var connectionSettings = new List<ConnectionSettingsDTO>();
            var configReader = new ConfigReader(FileName);

            foreach (var config in configReader.FileContents.Where(p => p.Contains($"kind: {EntityKind.Connection}")))
            {
                var parsed = YamlConverter.Deserialize<ConnectionSettingsDTO>(config);
                parsed.PartitionKey = token;
                parsed.TargetKind = targets.Any(p => p.Id == parsed.TargetId) ? EntityKind.Target : EntityKind.Connection;
                connectionSettings.Add(parsed);
            }

            foreach (var conn in connectionSettings)
            {
                api.UpdateConnection(conn);
            };

            return connectionSettings;
        }

        public IEnumerable<TargetDTO> InitTargets(string token, MessageSiloAPIService api)
        {
            var targets = new List<TargetDTO>();
            var configReader = new ConfigReader(FileName);

            foreach (var config in configReader.FileContents.Where(p => p.Contains($"kind: {EntityKind.Target}")))
            {
                var parsed = YamlConverter.Deserialize<TargetDTO>(config);
                parsed.PartitionKey = token;
                targets.Add(parsed);
            }

            foreach (var target in targets)
            {
                api.UpdateTarget(target);
            };

            return targets;
        }
    }
}
