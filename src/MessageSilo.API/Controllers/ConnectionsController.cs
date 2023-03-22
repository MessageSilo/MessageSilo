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

        private readonly IGeneralRepository repo;

        public ConnectionsController(ILogger<ConnectionsController> logger, IClusterClient client, IHttpContextAccessor httpContextAccessor, IMessageRepository<CorrectedMessage> messages, IGeneralRepository repo) : base(logger, httpContextAccessor, client)
        {
            this.messages = messages;
            this.repo = repo;
        }

        [HttpGet("User/{token}/Connections")]
        public async Task<IEnumerable<ConnectionState>> Index(string token)
        {
            var result = new List<ConnectionState>();

            var connectionIds = await repo.Query(EntityKind.Connection, token);

            foreach (var connId in connectionIds)
            {
                var conn = client!.GetGrain<IConnectionGrain>(connId);
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
            await repo.Delete(EntityKind.Connection, new[] { id });
        }

        [HttpGet("User/{token}/Connections/{name}")]
        public async Task<ConnectionState> Show(string token, string name)
        {
            var id = $"{token}|{name}";
            var connectionIds = await repo.Query(EntityKind.Connection, token);

            if (!connectionIds.Contains(id))
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
            await repo.Add(EntityKind.Connection, new[] { id });
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
