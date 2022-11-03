using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using SBMonitor.Core.Enums;
using SBMonitor.Core.Models;
using System.Security.Claims;

namespace SBMonitor.API.Controllers
{
    public class MessageMonitorController : MessageSiloControllerBase
    {
        public MessageMonitorController(ILogger<MessageMonitorController> logger, IGrainFactory grainFactory, IHttpContextAccessor httpContextAccessor)
            : base(logger, grainFactory, httpContextAccessor)
        {
        }

        [HttpPost]
        public async Task Upsert([FromBody] ConnectionProps conn)
        {
            return;
        }

        [HttpDelete]
        public async Task Delete([FromQuery] Guid id)
        {
            return;
        }

        [HttpGet]
        public async Task<IList<ConnectionProps>> Connections()
        {
            return null;
        }
    }
}