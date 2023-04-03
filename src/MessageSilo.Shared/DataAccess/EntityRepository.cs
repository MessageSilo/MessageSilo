using Azure.Data.Tables;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MessageSilo.Shared.DataAccess
{
    public class EntityRepository : IEntityRepository
    {
        private readonly TableClient client;

        public EntityRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("ConnectionStrings:GrainStateStorage");

            client = new TableClient(connectionString, "Entities");

            client.CreateIfNotExists();
        }

        public async Task Add(IEnumerable<Entity> entites)
        {
            var countOfEntities = await Count(entites.First().PartitionKey);

            if (countOfEntities >= 5)
                throw new Exception("Count of max. allowed entities reached! ==WIP: Only in BETA nad FREE tier==");

            foreach (var e in entites)
            {
                await client.UpsertEntityAsync<Entity>(e);
            }
        }

        public async Task Delete(string token, IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                await client.DeleteEntityAsync(token, name);
            }
        }

        public async Task<IEnumerable<Entity>> Query(EntityKind kind, string? token = null)
        {
            var result = token is null ?
                client.Query<Entity>(filter: $"Kind eq '{kind}'") :
                client.Query<Entity>(filter: $"PartitionKey eq '{token}' and Kind eq '{kind}'");

            return await Task.FromResult(result);
        }

        public async Task<int> Count(string? token = null)
        {
            var result = client.Query<Entity>(p => p.PartitionKey == token, select: new[] { "PartitionKey " });

            return await Task.FromResult(result.Count());
        }
    }
}
