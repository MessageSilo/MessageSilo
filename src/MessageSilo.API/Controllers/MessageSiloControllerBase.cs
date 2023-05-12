using MessageSilo.Shared.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Security.Claims;

namespace MessageSilo.API.Controllers
{
    [Authorize]
    public abstract class MessageSiloControllerBase : ControllerBase
    {
        protected readonly ILogger<MessageSiloControllerBase> logger;

        protected readonly IClusterClient? client;

        protected readonly IHttpContextAccessor httpContextAccessor;

        protected readonly string loggedInUserId;

        public MessageSiloControllerBase(ILogger<MessageSiloControllerBase> logger, IHttpContextAccessor httpContextAccessor, IClusterClient? client = null)
        {
            this.logger = logger;
            this.client = client;
            this.httpContextAccessor = httpContextAccessor;

            loggedInUserId = httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }
    }
}
