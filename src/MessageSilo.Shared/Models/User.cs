using Azure;
using Azure.Data.Tables;

namespace MessageSilo.Shared
{
    public class User : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public List<string> Connections { get; set; } = new List<string>();
    }
}
