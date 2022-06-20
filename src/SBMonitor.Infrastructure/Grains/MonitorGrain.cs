using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Orleans;
using SBMonitor.Core.Interfaces;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;
using SignalR.Orleans.Core;

namespace SBMonitor.Infrastructure.Grains
{
    public abstract class MonitorGrain<T> : Grain, IMonitorGrain<T> where T : ConnectionProps
    {
        protected ServiceBusClient _client;

        protected ServiceBusProcessor _processor;

        protected readonly ServiceBusProcessorOptions _options = new()
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock,
            AutoCompleteMessages = false,
        };

        protected ILogger<MonitorGrain<T>> _logger;

        private HubContext<IMessageMonitor> _hub;

        private long _lastMessageSequenceNumber;

        public T ConnectionProps { get; private set; }

        public IList<ServiceBusReceivedMessage> Messages { get; private set; } = new List<ServiceBusReceivedMessage>();

        public async Task<T> ConnectAsync(T props)
        {
            if (_processor != null)
                return ConnectionProps;

            ConnectionProps = props;

            _client = new ServiceBusClient(props.ConnectionString);

            _processor = CreateProcessor();

            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            ConnectionProps.StartedAt = DateTime.UtcNow;

            await _processor.StartProcessingAsync();

            _logger.LogDebug($"{props.Name} connected");

            return ConnectionProps;
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

            await _hub.Group(ConnectionProps.Id.ToString()).Send("ReceiveMessage", body);

            Messages.Add(arg.Message);

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
