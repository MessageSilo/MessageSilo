﻿using CommandLine;
using MessageSilo.Features.Enricher;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Serialization;

namespace MessageSilo.SiloCTL.Options
{
    [Verb("apply", HelpText = "Apply a configuration to an entity by file name or stdin.\r\n\r\nYAML formats are accepted.")]
    public class ApplyOptions : AuthorizedOptions
    {
        public ApplyOptions() : base()
        {
        }

        [Option('f', "filename", Required = true, HelpText = "Filename or directory to files to use to create the entities.")]
        public string FileName { get; set; }

        [Option('s', "scale", Required = false, HelpText = "How many instances of entities to run in parallel. The default value is 1.")]
        public int Scale { get; set; } = 1;

        public void Apply()
        {
            var configReader = new ConfigReader(FileName);
            ApplyDTO dto = new()
            {
                Scale = Scale
            };

            foreach (var config in configReader.FileContents.Where(p => p.Contains($"kind: {EntityKind.Target}")))
            {
                var parsed = YamlConverter.Deserialize<TargetDTO>(config);
                dto.Targets.Add(parsed);
            }

            foreach (var config in configReader.FileContents.Where(p => p.Contains($"kind: {EntityKind.Enricher}")))
            {
                var parsed = YamlConverter.Deserialize<EnricherDTO>(config);
                dto.Enrichers.Add(parsed);
            }

            foreach (var config in configReader.FileContents.Where(p => p.Contains($"kind: {EntityKind.Connection}")))
            {
                var parsed = YamlConverter.Deserialize<ConnectionSettingsDTO>(config);
                parsed.TargetKind = dto.Targets.Any(p => p.Id == parsed.TargetId) ? EntityKind.Target : EntityKind.Connection;
                dto.Connections.Add(parsed);
            }

            var errors = api.Apply(dto);

            if (errors is not null)
                foreach (var error in errors)
                {
                    Console.WriteLine($"Cannot apply changes on '{error.EntityName}' because the following errors:");

                    foreach (var failure in error.ValidationFailures)
                    {
                        Console.WriteLine($"\t- {failure.ErrorMessage}");
                    }

                    return;
                }

            Console.WriteLine($"Changes applied successfully!");
        }
    }
}
