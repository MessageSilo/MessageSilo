using ConsoleTables;

namespace MessageSilo.SiloCTL
{
    internal static class Tables
    {
        public static ConsoleTable ShowConnectionsTable = new ConsoleTable(new ConsoleTableOptions()
        {
            Columns = new[] { "NAME", "TYPE", "STATUS" },
            EnableCount = true,
            NumberAlignment = Alignment.Right
        });
    }
}
