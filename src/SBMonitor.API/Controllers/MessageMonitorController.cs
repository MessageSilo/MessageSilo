using Microsoft.AspNetCore.Mvc;
using Orleans;
using SBMonitor.Core.Enums;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;

namespace SBMonitor.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MessageMonitorController : ControllerBase
    {
        private readonly ILogger<MessageMonitorController> _logger;
        private readonly IGrainFactory _factory;

        public MessageMonitorController(ILogger<MessageMonitorController> logger, IGrainFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        [HttpPost]
        public async Task<ConnectionProps> Upsert([FromBody] ConnectionProps connectionProps)
        {
            var grain = _factory.GetGrain<IQueueMonitorGrain>(connectionProps.Id);
            return await grain.ConnectAsync(connectionProps);
        }
    }
}