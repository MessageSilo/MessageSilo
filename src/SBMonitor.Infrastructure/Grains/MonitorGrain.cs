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
        protected ServiceBusClient client;

        protected ServiceBusProcessor processor;

        protected readonly ServiceBusProcessorOptions options = new()
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock,
            AutoCompleteMessages = false,
        };


        protected ILogger<MonitorGrain> _logger;

        private HubContext<IMessageMonitor> _hub;

        private long _lastMessageSequenceNumber;

        protected ConnectionProps connectionProps;

        public async Task ConnectAsync(ConnectionProps props)
        {
            if (processor != null)
                return;

            connectionProps = props;

            client = new ServiceBusClient(props.ConnectionString);

            processor = CreateProcessor();

            processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            await processor.StartProcessingAsync();

            _logger.LogDebug($"{props.Name} connected");
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception.Message);
            return Task.CompletedTask;
        }

        private async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            if (arg.Message.SequenceNumber <= _lastMessageSequenceNumber)
                return;

            string body = arg.Message.Body.ToString();

            _logger.LogDebug(body);
            await _hub.Group(this.GetPrimaryKey().ToString()).Send("ReceiveMessage", body);

            _lastMessageSequenceNumber = arg.Message.SequenceNumber;

            await arg.AbandonMessageAsync(arg.Message);
        }

        protected abstract ServiceBusProcessor CreateProcessor();

        public override async Task OnActivateAsync()
        {
            _hub = GrainFactory.GetHub<IMessageMonitor>();
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await processor.DisposeAsync();
            await client.DisposeAsync();
            await base.OnDeactivateAsync();
        }
    }
}
