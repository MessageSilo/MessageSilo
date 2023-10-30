using MessageSilo.Features.Target;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;

namespace MessageSilo.Controllers
{
    public class TargetsController : CRUDController<TargetDTO, TargetDTO, ITargetGrain>
    {
        protected override EntityKind GetKind() => EntityKind.Target;

        public TargetsController(
            ILogger<CRUDController<TargetDTO, TargetDTO, ITargetGrain>> logger,
            IClusterClient client,
            IHttpContextAccessor httpContextAccessor) :
            base(logger, client, httpContextAccessor)
        {
        }
    }
}
