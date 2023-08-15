using MessageSilo.Features.Connection;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Orleans;

namespace MessageSilo.API.Controllers
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
    }
}
