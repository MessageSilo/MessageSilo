using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;
using MessageSilo.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MessageSilo.Controllers
{
    [Route("api/v1/[controller]")]
    public class ConnectionsController : MessageSiloControllerBase
    {
        public ConnectionsController(
            ILogger<ConnectionsController> logger,
            IHttpContextAccessor httpContextAccessor,
            IClusterClient client) : base(logger, httpContextAccessor, client)
        {
        }

        [HttpPost("{name}")]
        public async Task Send(string name, [FromBody] MessageDTO dto)
        {
            var conn = await entityManagerGrain.GetConnectionSettings(name);
            var grain = client.GetGrain<IConnectionGrain>($"{conn.Id}#{1}");

            await grain.Send(new Message(dto.Id, dto.Body));
        }

    }
}
