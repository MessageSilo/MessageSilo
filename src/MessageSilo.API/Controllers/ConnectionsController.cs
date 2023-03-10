using MessageSilo.Features.Connection;
using MessageSilo.Features.MessageCorrector;
using MessageSilo.Shared.DataAccess;
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
        public async Task<IEnumerable<string>> Index(string token)
        {
            return await repo.QueryConnections(token);
        }

        [HttpDelete("/Connections/{id}")]
        public async Task Delete(string id)
        {
            var conn = client!.GetGrain<IConnectionGrain>(id);
            await conn.Delete();
            await repo.DeleteConnections(new[] { id });
        }

        [HttpGet("Connections/{id}")]
        public async Task<ConnectionState> Show(string id)
        {
            var conn = client!.GetGrain<IConnectionGrain>(id);

            return await conn.GetState();
        }

        [HttpPut("Connections/{id}")]
        public async Task<ConnectionState> Update(string id, [FromBody] ConnectionSettingsDTO dto)
        {
            await repo.AddConnections(new[] { dto.Id });
            var conn = client!.GetGrain<IConnectionGrain>(id);
            await conn.Update(dto);

            return await conn.GetState();
        }

        [HttpGet("Connections/{id}/Messages")]
        public async Task<IEnumerable<CorrectedMessage>> ShowMessages(string id, [FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset to)
        {
            return await messages.Query(id, from, to);
        }
    }
}
