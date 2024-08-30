using CommandLine;
using ConsoleTables;
using MessageSilo.Infrastructure.Interfaces;

namespace MessageSilo.SiloCTL.Options
{
    [Verb("show", HelpText = "Display one or many entities.\r\n\r\nPrints a table of the most important information about the specified entities.")]
    public class ShowOptions : Options
    {
        private readonly ConsoleTable showEntitesTable = new(new ConsoleTableOptions()
        {
            Columns = ["KIND", "NAME"],
            EnableCount = true,
            NumberAlignment = Alignment.Right
        });

        [Option('n', "name", Required = false, HelpText = "Display detailed information about a specific entity.")]
        public string Name { get; set; }

        public ShowOptions() : base()
        {
        }

        public void Show(IMessageSiloAPI api)
        {
            var entities = api.List().GetAwaiter().GetResult();

            if (!string.IsNullOrEmpty(Name))
            {
                var entity = entities.FirstOrDefault(p => p.Name == Name);

                if (entity is not null)
                    Console.WriteLine(entity.YamlDefinition);
                else
                    Console.WriteLine($"Entity '{Name}' not found.");

                return;
            }

            if (!entities.Any())
            {
                Console.WriteLine("No entities found.");
                return;
            }

            foreach (var entity in entities)
                showEntitesTable.AddRow(entity.Kind, entity.Name);

            showEntitesTable.Write(Format.Default);
        }
    }
}
