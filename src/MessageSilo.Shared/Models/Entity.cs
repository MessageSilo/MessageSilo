using Azure;
using Azure.Data.Tables;
using InfluxDB.Client.Api.Domain;
using MessageSilo.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core.Tokens;
using YamlDotNet.Serialization;

namespace MessageSilo.Shared.Models
{
    public class Entity : ITableEntity, IComparable<Entity>
    {
        [YamlMember(Alias = "userId")]
        public string PartitionKey { get; set; }

        [YamlMember(Alias = "name")]
        public string RowKey { get; set; }

        public EntityKind Kind { get; set; }

        public string Id => $"{PartitionKey}|{RowKey}";

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public int CompareTo(Entity? other)
        {
            var result = this.Id.CompareTo(other?.Id);

            if (result == 0)
                result = this.Kind.CompareTo(other?.Kind);

            return result;
        }
    }
}
