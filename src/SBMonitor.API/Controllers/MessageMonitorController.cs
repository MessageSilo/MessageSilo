using Microsoft.AspNetCore.Mvc;
using Orleans;
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
        public async Task ConnectToQueue([FromBody] QueueConnectionProps connectionProps)
        {
            var grain = _factory.GetGrain<IMonitorGrain<QueueConnectionProps>>(Guid.Empty);
            grain.Connect(connectionProps);
        }

        [HttpPost]
        public async Task ConnectToTopic([FromBody] TopicConnectionProps connectionProps)
        {
            var grain = _factory.GetGrain<IMonitorGrain<TopicConnectionProps>>(Guid.Empty);
            grain.Connect(connectionProps);
        }
    }
}