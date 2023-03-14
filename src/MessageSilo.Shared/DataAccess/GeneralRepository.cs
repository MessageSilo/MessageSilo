using Azure.Data.Tables;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace MessageSilo.Shared.DataAccess
{
    public class GeneralRepository : IGeneralRepository
    {
        private const string CONNECTIONS_TABLE_NAME = "Connections";

        private readonly TableClient tableClient;

        public GeneralRepository(IConfiguration configuration)
        {
            var connectionString = configuration.GetValue<string>("ConnectionStrings:GrainStateStorage");

            tableClient = new TableClient(connectionString, CONNECTIONS_TABLE_NAME);
            tableClient.CreateIfNotExists();
        }

        public async Task AddConnections(IEnumerable<string> connectionIds)
        {
            foreach (var connId in connectionIds)
            {
                var splitted = connId.Split("|");
                var entity = new TableEntity(splitted[0], connId);
                await tableClient.UpsertEntityAsync(entity);
            }
        }

        public async Task DeleteConnections(IEnumerable<string> connectionIds)
        {
            foreach (var connId in connectionIds)
            {
                var splitted = connId.Split("|");
                await tableClient.DeleteEntityAsync(splitted[0], connId);
            }
        }

        public async Task<IEnumerable<string>> QueryConnections(string? token = null)
        {
            var result = token is null ? tableClient.Query<TableEntity>() : tableClient.Query<TableEntity>(p => p.PartitionKey == token);

            return await Task.FromResult(result.Select(p => p.RowKey));
        }
    }
}
