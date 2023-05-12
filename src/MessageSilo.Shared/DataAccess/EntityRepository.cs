using Azure.Data.Tables;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

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
                throw new Exception("Count of max. allowed entities reached! ==WIP: Only in BETA and FREE tier==");

            foreach (var e in entites)
            {
                await client.UpsertEntityAsync<Entity>(e);
            }
        }

        public async Task Delete(string userId, IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                await client.DeleteEntityAsync(userId, name);
            }
        }

        public async Task<IEnumerable<Entity>> Query(EntityKind? kind = null, string? userId = null)
        {
            var kindFilter = $"Kind eq '{kind}'";
            var userIdFilter = $"PartitionKey eq '{userId}'";

            var filters = new List<string>();

            if (kind is not null)
                filters.Add(kindFilter);

            if (userId is not null)
                filters.Add(userIdFilter);

            var result = filters.Count == 0 ?
                client.Query<Entity>() :
                client.Query<Entity>(filter: string.Join(" and ", filters));

            return await Task.FromResult(result);
        }

        public async Task<int> Count(string? userId = null)
        {
            var result = client.Query<Entity>(p => p.PartitionKey == userId, select: new[] { "PartitionKey " });

            return await Task.FromResult(result.Count());
        }
    }
}
