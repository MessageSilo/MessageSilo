using Azure.Data.Tables;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Configuration;

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

        public async Task Upsert(Entity entity)
        {
            await client.UpsertEntityAsync<Entity>(entity);
        }

        public async Task Delete(string userId, string entityName)
        {
            await client.DeleteEntityAsync(userId, entityName);
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
    }
}
