using MessageSilo.Features.Connection;
using MessageSilo.Features.MessageCorrector;
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
        private readonly IMessageRepository<CorrectedMessage> messages;

        public ConnectionsController(ILogger<ConnectionsController> logger, IClusterClient client, IHttpContextAccessor httpContextAccessor, IMessageRepository<CorrectedMessage> messages, IEntityRepository repo)
            : base(logger, httpContextAccessor, client, repo)
        {
            this.messages = messages;
        }

        [HttpGet("User/{token}/Connections")]
        public async Task<IEnumerable<ConnectionState>> Index(string token)
        {
            var result = new List<ConnectionState>();

            var connections = await repo.Query(EntityKind.Connection, token);

            foreach (var c in connections)
            {
                var conn = client!.GetGrain<IConnectionGrain>(c.Id);
                result.Add(await conn.GetState());
            }

            return result;
        }

        [HttpDelete("User/{token}/Connections/{name}")]
        public async Task Delete(string token, string name)
        {
            var id = $"{token}|{name}";
            var conn = client!.GetGrain<IConnectionGrain>(id);
            await conn.Delete();
            await repo.Delete(token, new[] { name });
        }

        [HttpGet("User/{token}/Connections/{name}")]
        public async Task<ConnectionState> Show(string token, string name)
        {
            var id = $"{token}|{name}";
            var connections = await repo.Query(EntityKind.Connection, token);

            if (!connections.Any(p => p.Id == id))
            {
                httpContextAccessor!.HttpContext!.Response.StatusCode = StatusCodes.Status404NotFound;
                return null!;
            }

            var conn = client!.GetGrain<IConnectionGrain>(id);

            return await conn.GetState();
        }

        [HttpPut("User/{token}/Connections/{name}")]
        public async Task<ConnectionState> Update(string token, string name, [FromBody] ConnectionSettingsDTO dto)
        {
            var id = $"{token}|{name}";

            await repo.Add(new[]
            {
                new Entity()
                {
                    PartitionKey = token,
                    RowKey = name,
                    Kind = EntityKind.Connection
                }
            });

            var conn = client!.GetGrain<IConnectionGrain>(id);
            await conn.Update(dto);

            return await conn.GetState();
        }

        [HttpGet("User/{token}/Connections/{name}/Messages")]
        public async Task<IEnumerable<CorrectedMessage>> ShowMessages(string token, string name, [FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset to)
        {
            var id = $"{token}|{name}";
            return await messages.Query(id, from, to);
        }
    }
}
