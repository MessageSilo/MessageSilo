using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using SBMonitor.Core.Interfaces;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;
using SignalR.Orleans.Core;

namespace SBMonitor.Infrastructure.Grains
{
    public abstract class MonitorGrain : Grain, IMonitorGrain
    {
        private HubContext<IMessageMonitor> hub;

        private IDisposable timer;

        protected ConnectionProps connectionProps;

        protected ServiceBusClient client;

        protected ServiceBusReceiver receiver;

        protected ILogger<MonitorGrain> logger;

        public Task ConnectAsync(ConnectionProps props)
        {
            if (receiver != null)
                return Task.CompletedTask;

            connectionProps = props;

            client = new ServiceBusClient(props.ConnectionString);

            receiver = CreateReceiver();

            timer = RegisterTimer(processMessageAsync, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        private async Task processMessageAsync(object state)
        {
            var msg = await receiver.PeekMessageAsync();

            if (msg == null)
                return;

            string body = msg.Body.ToString();

            logger.LogDebug(body);
            await hub.Group(this.GetPrimaryKey().ToString()).Send("ReceiveMessage", body);
        }

        protected abstract ServiceBusReceiver CreateReceiver();

        public override async Task OnActivateAsync()
        {
            hub = GrainFactory.GetHub<IMessageMonitor>();
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            timer.Dispose();
            await receiver.DisposeAsync();
            await client.DisposeAsync();
            await base.OnDeactivateAsync();
        }
    }
}
