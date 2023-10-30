using MessageSilo.Features.Connection;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MessageSilo.Controllers
{
    public class ConnectionsController : CRUDController<ConnectionSettingsDTO, ConnectionState, IConnectionGrain>
    {
        protected override EntityKind GetKind() => EntityKind.Connection;

        public ConnectionsController(
            ILogger<CRUDController<ConnectionSettingsDTO, ConnectionState, IConnectionGrain>> logger,
            IClusterClient client,
            IHttpContextAccessor httpContextAccessor) :
            base(logger, client, httpContextAccessor)
        {
        }

        [HttpPost(template: "{name}/send")]
        public async Task<ApiContract<string>> Send(string name, [FromBody] dynamic message)
        {
            var id = $"{loggedInUserId}|{name}";

            var conn = client!.GetGrain<IConnectionGrain>(id);
            await conn.Send(new Message(Guid.NewGuid().ToString(), JsonSerializer.Serialize(message)));

            return new ApiContract<string>(httpContextAccessor, StatusCodes.Status200OK, data: null);
        }
    }
}
