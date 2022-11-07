using Orleans;

namespace MessageSilo.API.Controllers
{
    public class DeadLetterCorrectorController : MessageSiloControllerBase
    {
        public DeadLetterCorrectorController(ILogger<DeadLetterCorrectorController> logger, IClusterClient client, IHttpContextAccessor httpContextAccessor) : base(logger, client, httpContextAccessor)
        {
        }
    }
}
