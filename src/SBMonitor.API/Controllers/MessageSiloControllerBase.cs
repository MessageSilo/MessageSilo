using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Security.Claims;

namespace SBMonitor.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public abstract class MessageSiloControllerBase : ControllerBase
    {
        protected readonly ILogger<MessageMonitorController> logger;

        protected readonly IGrainFactory grainFactory;

        protected readonly IHttpContextAccessor httpContextAccessor;

        protected readonly string loggedInUserId;

        public MessageSiloControllerBase(ILogger<MessageMonitorController> logger, IGrainFactory grainFactory, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.grainFactory = grainFactory;
            this.httpContextAccessor = httpContextAccessor;

            loggedInUserId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
