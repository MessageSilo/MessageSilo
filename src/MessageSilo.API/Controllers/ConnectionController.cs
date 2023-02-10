using MessageSilo.Features.MessageCorrector;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Grains;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace MessageSilo.API.Controllers
{
    [Route("api/v1")]
    public class ConnectionController : MessageSiloControllerBase
    {
        private readonly IUserGrain user;

        private readonly IMessageRepository<CorrectedMessage> messages;

        public ConnectionController(ILogger<ConnectionController> logger, IClusterClient client, IHttpContextAccessor httpContextAccessor, IMessageRepository<CorrectedMessage> messages) : base(logger, client, httpContextAccessor)
        {
            user = client.GetGrain<IUserGrain>(loggedInUserId);
            this.messages = messages;
        }

        [HttpGet("Connection")]
        public async Task<List<ConnectionSettingsDTO>> List()
        {
            await user.InitConnections();
            return await user.GetConnections();
        }

        [HttpGet("Connection/{id}")]
        public async Task<ConnectionSettingsDTO?> Find(Guid id)
        {
            return (await user.GetConnections()).FirstOrDefault(p => p.Id == id);
        }

        [HttpPut("Connection")]
        public async Task Upsert([FromBody] ConnectionSettingsDTO dto)
        {
            await user.AddConnection(dto);
        }

        [HttpGet("Connection/{id}/Messages")]
        public async Task<IEnumerable<CorrectedMessage>> Messages(Guid id, [FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset to)
        {
            Request.Query.ToString();
            return await messages.Query(id.ToString(), from, to);
        }

        [HttpDelete("Connection/{id}")]
        public async Task Delete(Guid id)
        {
            await user.DeleteConnection(id);
        }
    }
}
