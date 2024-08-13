using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;
using MessageSilo.Features.Connection;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MessageSilo.Features.RabbitMQ
{
    public class RabbitMQConnectionGrain : MessagePlatformConnectionGrain, IRabbitMQConnectionGrain
    {
        private readonly ILogger<RabbitMQConnectionGrain> logger;

        private readonly IGrainFactory grainFactory;

        private IConnection connection;

        private IModel channel;


        public RabbitMQConnectionGrain(ILogger<RabbitMQConnectionGrain> logger, IGrainFactory grainFactory)
        {
            this.logger = logger;
            this.grainFactory = grainFactory;
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken token)
        {
            var grain = grainFactory.GetGrain<IRabbitMQConnectionGrain>(this.GetPrimaryKeyString());

            grain.GetPrimaryKeyString();

            return Task.CompletedTask;
        }

        public override async ValueTask DisposeAsync()
        {
            if (channel is not null)
                channel.Dispose();

            if (connection is not null)
                connection.Dispose();

            await Task.CompletedTask;
        }

        public override async Task Enqueue(Message message)
        {
            channel.BasicPublish(exchange: settings.ExchangeName ?? string.Empty,
                     routingKey: settings.ExchangeName is null ? settings.QueueName : string.Empty,
                     basicProperties: channel.CreateBasicProperties(),
                     body: Encoding.UTF8.GetBytes(message.Body));

            await Task.CompletedTask;
        }

        public override async Task Init(ConnectionSettingsDTO settings)
        {
            this.settings = settings;

            var factory = new ConnectionFactory
            {
                Uri = new Uri(this.settings.ConnectionString),
                DispatchConsumersAsync = true
            };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            if (this.settings.ReceiveMode != ReceiveMode.None)
            {
                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.Received += async (model, ea) =>
                {
                    string body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var messageId = ea.BasicProperties.MessageId ?? Guid.NewGuid().ToString();

                    var connection = grainFactory.GetGrain<IConnectionGrain>(this.GetPrimaryKeyString());

                    var isDelivered = await connection.TransformAndSend(new Message(messageId, body));

                    if (isDelivered && this.settings.ReceiveMode == ReceiveMode.ReceiveAndDelete)
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                channel.BasicConsume(queue: this.settings.QueueName,
                                     consumer: consumer);

            }

            await Task.CompletedTask;
        }
    }
}
