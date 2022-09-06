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

        public async Task AddOrUpdateConnection(ConnectionProps conn)
        {
            var existingConnection = User.State.Connections.FirstOrDefault(p => p.Id == conn.Id);

            if (existingConnection == null)
                User.State.Connections.Add(conn);
            else
                existingConnection.Update(conn);

            //await User.WriteStateAsync();
        }

        public async Task RemoveConnection(Guid id)
        {
            var removable = User.State.Connections.First(p => p.Id == id);
            User.State.Connections.Remove(removable);
            await User.WriteStateAsync();
        }

        public async Task<IList<ConnectionProps>> Connections()
        {
            await User.ReadStateAsync();
            return User.State.Connections;
        }
    }
}
