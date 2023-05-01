using MessageSilo.Features.Connection;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace MessageSilo.API.Controllers
{
    [Route("api/v1")]
    public class ConnectionsController : MessageSiloControllerBase
    {
        public ConnectionsController(ILogger<ConnectionsController> logger, IClusterClient client, IHttpContextAccessor httpContextAccessor, IEntityRepository repo)
            : base(logger, httpContextAccessor, client, repo)
        {
        }

        [HttpGet("Connections")]
        public async Task<IEnumerable<ConnectionState>> Index()
        {
            var result = new List<ConnectionState>();

            var connections = await repo.Query(EntityKind.Connection, loggedInUserId);

            foreach (var c in connections)
            {
                var conn = client!.GetGrain<IConnectionGrain>(c.Id);
                result.Add(await conn.GetState());
            }

            return result;
        }

        [HttpDelete("Connections/{name}")]
        public async Task Delete(string name)
        {
            var id = $"{loggedInUserId}|{name}";
            var conn = client!.GetGrain<IConnectionGrain>(id);
            await conn.Delete();
            await repo.Delete(loggedInUserId, new[] { name });
        }

        [HttpGet("Connections/{name}")]
        public async Task<ConnectionState> Show(string name)
        {
            var id = $"{loggedInUserId}|{name}";
            var connections = await repo.Query(EntityKind.Connection, loggedInUserId);

            if (!connections.Any(p => p.Id == id))
            {
                httpContextAccessor!.HttpContext!.Response.StatusCode = StatusCodes.Status404NotFound;
                return null!;
            }

            var conn = client!.GetGrain<IConnectionGrain>(id);

            return await conn.GetState();
        }

        [HttpPut("Connections/{name}")]
        public async Task<ConnectionState> Update(string name, [FromBody] ConnectionSettingsDTO dto)
        {
            dto.PartitionKey = loggedInUserId;

            await repo.Add(new[]
            {
                new Entity()
                {
                    PartitionKey = loggedInUserId,
                    RowKey = name,
                    Kind = EntityKind.Connection
                }
            });

            var conn = client!.GetGrain<IConnectionGrain>(dto.Id);
            await conn.Update(dto);

            return await conn.GetState();
        }
    }
}
