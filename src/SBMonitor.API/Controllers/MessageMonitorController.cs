using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using SBMonitor.Core.Enums;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;
using System.Security.Claims;

namespace SBMonitor.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class MessageMonitorController : ControllerBase
    {
        private readonly ILogger<MessageMonitorController> _logger;
        private readonly IGrainFactory _factory;
        private readonly IHttpContextAccessor httpContextAccessor;

        public MessageMonitorController(ILogger<MessageMonitorController> logger, IGrainFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _factory = factory;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<ConnectionProps> Upsert([FromBody] ConnectionProps connectionProps)
        {
            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            IMonitorGrain monitor;
            
            switch (connectionProps.TypeOfBus)
            {
                case BusType.Queue:
                    monitor = _factory.GetGrain<IQueueMonitorGrain>(connectionProps.Id);
                    break;
                case BusType.Topic:
                default:
                    monitor = _factory.GetGrain<ITopicMonitorGrain>(connectionProps.Id);
                    break;
            }

            var result = await monitor.ConnectAsync(connectionProps);

            var user = _factory.GetGrain<IUserGrain>(userId);

            await user.AddMonitorGrain(connectionProps.Id);

            return result;
        }
    }
}