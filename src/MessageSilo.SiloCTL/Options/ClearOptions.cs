using CommandLine;

namespace MessageSilo.SiloCTL.Options
{
    [Verb("clear", HelpText = "Clear all entities.")]
    public class ClearOptions : Options
    {
        public ClearOptions() : base()
        {
        }

        public void Clear(MessageSiloAPI api)
        {
            api.Clear();

            Console.WriteLine($"Cleared successfully!");
        }
    }
}
