using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Infrastructure.Grains
{
    public class UserGrain : Grain, IUserGrain
    {
        protected ILogger<UserGrain> _logger;

        protected IPersistentState<User> User { get; set; }

        public UserGrain(ILogger<UserGrain> logger, [PersistentState("userGrainState")] IPersistentState<User> user) : base()
        {
            _logger = logger;
            User = user;
        }

        public override Task OnActivateAsync()
        {
            return Task.CompletedTask;
        }

        public async Task AddMonitorGrain(Guid id)
        {
            User.State.MonitorGrainIds.Add(id);
            await User.WriteStateAsync();
        }

        public async Task RemoveMonitorGrain(Guid id)
        {
            User.State.MonitorGrainIds.Remove(id);
            await User.WriteStateAsync();
        }
    }
}
