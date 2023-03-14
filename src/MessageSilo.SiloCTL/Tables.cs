using ConsoleTables;
using InfluxDB.Client.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
