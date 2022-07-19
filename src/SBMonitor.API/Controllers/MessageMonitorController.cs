using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using SBMonitor.Core.Enums;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains;
using SBMonitor.Infrastructure.Grains.Interfaces;
using System.Security.Claims;
using SBMonitor.Infrastructure.Extensions;

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
            var monitor = grainFactory.MonitorGrain(conn.TypeOfBus, conn.Id);
            await monitor.ConnectAsync(conn);

            var user = grainFactory.GetGrain<IUserGrain>(loggedInUserId);
            await user.AddOrUpdateConnection(conn);
        }

        [HttpDelete]
        public async Task Delete([FromQuery] Guid id)
        {
            var user = grainFactory.GetGrain<IUserGrain>(loggedInUserId);

            await user.RemoveConnection(id);
        }

        [HttpGet]
        public async Task<IList<ConnectionProps>> Connections()
        {
            var user = grainFactory.GetGrain<IUserGrain>(loggedInUserId);

            var result = new List<ConnectionProps>();

            foreach (var conn in await user.Connections())
            {
                var monitor = grainFactory.MonitorGrain(conn.TypeOfBus, conn.Id);
                await monitor.ConnectAsync(conn);
                result.Add(conn);
            }

            return result;
        }
    }
}