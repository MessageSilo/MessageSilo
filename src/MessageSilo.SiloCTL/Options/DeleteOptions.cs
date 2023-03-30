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
    [Verb("delete", HelpText = "Apply a configuration to an entity by file name or stdin.\r\n\r\nThe entity name must be specified. This entity will be created if it doesn't exist yet.\r\nYAML formats are accepted.")]
    public class DeleteOptions
    {
        [Option('n', "name", Required = true, HelpText = "Deletes a specific entity.")]
        public string Name { get; set; }

        public void Delete(string token, MessageSiloAPIService api)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                api.DeleteConnection(token, Name);
            }
        }
    }
}
