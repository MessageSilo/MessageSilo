using Azure.Data.Tables;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MessageSilo.Shared.DataAccess
{
    public class GeneralRepository : IGeneralRepository
    {
        private readonly IDictionary<EntityKind, TableClient> clients;

        public GeneralRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("ConnectionStrings:GrainStateStorage");

            clients = new Dictionary<EntityKind, TableClient>()
            {
                { EntityKind.Connection, new TableClient(connectionString, "Connections") },
                { EntityKind.Target, new TableClient(connectionString, "Targets") }
            };

            foreach (var c in clients)
                c.Value.CreateIfNotExists();
        }

        public async Task Add(EntityKind kind, IEnumerable<string> ids)
        {
            foreach (var id in ids)
            {
                var splitted = id.Split("|");
                var entity = new TableEntity(splitted[0], id);
                await clients[kind].UpsertEntityAsync(entity);
            }
        }

        public async Task Delete(EntityKind kind, IEnumerable<string> ids)
        {
            foreach (var id in ids)
            {
                var splitted = id.Split("|");
                await clients[kind].DeleteEntityAsync(splitted[0], id);
            }
        }

        public async Task<IEnumerable<string>> Query(EntityKind kind, string? token = null)
        {
            var result = token is null ? clients[kind].Query<TableEntity>() : clients[kind].Query<TableEntity>(p => p.PartitionKey == token);

            return await Task.FromResult(result.Select(p => p.RowKey));
        }
    }
}
