using CommandLine;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;

namespace MessageSilo.SiloCTL.Options
{
    [Verb("clear", HelpText = "Clear all entities.")]
    public class ClearOptions : AuthorizedOptions
    {
        public ClearOptions() : base()
        {
        }

        public void Clear()
        {
            api.Clear();

            Console.WriteLine($"Cleared successfully!");
        }
    }
}
