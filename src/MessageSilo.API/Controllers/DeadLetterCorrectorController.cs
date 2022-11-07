using Orleans;

namespace MessageSilo.API.Controllers
{
    public class DeadLetterCorrectorController : MessageSiloControllerBase
    {
        private readonly IClusterClient client;

        public DeadLetterCorrectorController(ILogger logger, IGrainFactory grainFactory, IHttpContextAccessor httpContextAccessor, IClusterClient client) : base(logger, grainFactory, httpContextAccessor)
        {
            this.client = client;
        }
    }
}
