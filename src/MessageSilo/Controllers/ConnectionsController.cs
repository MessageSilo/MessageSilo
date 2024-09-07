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

        [HttpPost("{id}")]
        public async Task Send(string id, [FromBody] MessageDTO dto)
        {
            var grain = client.GetGrain<IConnectionGrain>(id);

            await grain.Send(new Message(dto.Id, dto.Body));
        }

    }
}
