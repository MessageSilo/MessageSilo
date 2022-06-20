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
        public async Task<QueueConnectionProps> ConnectToQueue([FromBody] QueueConnectionProps connectionProps)
        {
            var grain = _factory.GetGrain<IMonitorGrain<QueueConnectionProps>>(connectionProps.Id);
            return await grain.ConnectAsync(connectionProps);
        }

        [HttpPost]
        public async Task<TopicConnectionProps> ConnectToTopic([FromBody] TopicConnectionProps connectionProps)
        {
            var grain = _factory.GetGrain<IMonitorGrain<TopicConnectionProps>>(connectionProps.Id);
            return await grain.ConnectAsync(connectionProps);
        }

        [HttpGet]
        public IReadOnlyList<ConnectionProps> Init()
        {
            var cmGrain = _factory.GetGrain<IConnectionManagerGrain>(Guid.Empty);
            var monitorGrains = cmGrain.List();

            return monitorGrains.Select(p => p.ConnectionProps).ToList();
        }
    }
}