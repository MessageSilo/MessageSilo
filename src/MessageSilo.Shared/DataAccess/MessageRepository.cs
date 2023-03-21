using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Core.Flux.Domain;
using InfluxDB.Client.Writes;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Shared.DataAccess
{
    public class MessageRepository<T> : IMessageRepository<T> where T : Message
    {
        private const string ORGANIZATION = "MessageSilo";

        private readonly string token;
        private readonly string url;
        private readonly string bucket;

        public MessageRepository(IConfiguration configuration)
        {
            token = configuration.GetValue<string>("DeadLetterMessageRepository:Token");
            url = configuration.GetValue<string>("DeadLetterMessageRepository:Url");
            bucket = configuration.GetValue<string>("DeadLetterMessageRepository:Bucket");
        }

        public void Add(string connectionId, T message)
        {
            write(write =>
            {
                var point = PointData.Measurement(connectionId)
                    .Field("message", JsonConvert.SerializeObject(message))
                    .Timestamp(DateTime.UtcNow, WritePrecision.Ns);

                write.WritePoint(point, bucket, ORGANIZATION);
            });
        }

        public async Task<IEnumerable<T>> Query(string connectionId, DateTimeOffset from, DateTimeOffset to)
        {
            var result = await queryAsync(async query =>
            {
                var flux = $"from(bucket:\"{bucket}\") |> range(start: {from.ToUnixTimeSeconds()}, stop: {to.ToUnixTimeSeconds()}) |> filter(fn: (r) => r._measurement == \"{connectionId}\")";

                var tables = await query.QueryAsync(flux, ORGANIZATION);

                return tables.SelectMany(table =>
                    table.Records.Select(r => r.GetValue().ToString()!)
                    );
            });

            return result.Select(p => JsonConvert.DeserializeObject<T>(p)!);
        }

        private void write(Action<WriteApi> action)
        {
            using var client = new InfluxDBClient(url, token);
            using var write = client.GetWriteApi();
            action(write);
        }

        private async Task<IEnumerable<string>> queryAsync(Func<QueryApi, Task<IEnumerable<string>>> action)
        {
            using var client = new InfluxDBClient(url, token);
            var query = client.GetQueryApi();
            return await action(query);
        }
    }
}
