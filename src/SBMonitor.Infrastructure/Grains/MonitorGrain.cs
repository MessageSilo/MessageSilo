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
        protected ServiceBusClient _client;

        protected ServiceBusProcessor _processor;

        protected readonly ServiceBusProcessorOptions _options = new()
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock,
            AutoCompleteMessages = false,
        };


        protected ILogger<MonitorGrain> _logger;

        private HubContext<IMessageMonitor> _hub;

        private long _lastMessageSequenceNumber;

        public IPersistentState<ConnectionProps> ConnectionProps { get; protected set; }

        public async Task<ConnectionProps> ConnectAsync(ConnectionProps props)
        {
            if (_processor != null)
                return ConnectionProps.State;

            ConnectionProps.State = props;

            _client = new ServiceBusClient(props.ConnectionString);

            _processor = CreateProcessor();

            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            ConnectionProps.State.StartedAt = DateTime.UtcNow;
            await ConnectionProps.WriteStateAsync();

            await _processor.StartProcessingAsync();

            _logger.LogDebug($"{props.Name} connected");

            return ConnectionProps.State;
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
            await _hub.Group(this.GetPrimaryKeyString()).Send("ReceiveMessage", body);

            _lastMessageSequenceNumber = arg.Message.SequenceNumber;
        }

        protected abstract ServiceBusProcessor CreateProcessor();

        public override Task OnActivateAsync()
        {
            _hub = GrainFactory.GetHub<IMessageMonitor>();
            return Task.CompletedTask;
        }
    }
}
