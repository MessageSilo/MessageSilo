using FluentValidation;
using MessageSilo.Features.Enricher;
using MessageSilo.Features.EntityManager;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Orleans;

namespace MessageSilo.API.Controllers
{
    public class EnrichersController : CRUDController<EnricherDTO, EnricherDTO, IEnricherGrain>
    {
        protected override EntityKind GetKind() => EntityKind.Enricher;

        public EnrichersController(
            ILogger<CRUDController<EnricherDTO, EnricherDTO, IEnricherGrain>> logger,
            IClusterClient client,
            IHttpContextAccessor httpContextAccessor,
            IEntityRepository repo) :
            base(logger, client, httpContextAccessor, repo)
        {
        }
    }
}
