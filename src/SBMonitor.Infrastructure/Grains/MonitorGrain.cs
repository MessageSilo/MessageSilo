using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Orleans;
using SBMonitor.Core.Models;
using SBMonitor.Infrastructure.Grains.Interfaces;

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

        public T ConnectionProps { get; private set; }

        public void Connect(T props)
        {
            if (_processor != null)
                return;

            ConnectionProps = props;

            _client = new ServiceBusClient(props.ConnectionString);

            _processor = CreateProcessor();

            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            _processor.StartProcessingAsync();

            _logger.LogDebug($"{props.Name} connected");
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception.Message);
            return Task.CompletedTask;
        }

        private Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            string body = arg.Message.Body.ToString();
            _logger.LogDebug(body);

            return Task.CompletedTask;
        }

        protected abstract ServiceBusProcessor CreateProcessor();
    }
}
