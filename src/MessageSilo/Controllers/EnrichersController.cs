using MessageSilo.Features.Enricher;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;

namespace MessageSilo.Controllers
{
    public class EnrichersController : CRUDController<EnricherDTO, EnricherDTO, IEnricherGrain>
    {
        protected override EntityKind GetKind() => EntityKind.Enricher;

        public EnrichersController(
            ILogger<CRUDController<EnricherDTO, EnricherDTO, IEnricherGrain>> logger,
            IClusterClient client,
            IHttpContextAccessor httpContextAccessor) :
            base(logger, client, httpContextAccessor)
        {
        }
    }
}
