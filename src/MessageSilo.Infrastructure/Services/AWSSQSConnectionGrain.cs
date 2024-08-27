using Amazon.SQS;
using Amazon.SQS.Model;
using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Enums;
using MessageSilo.Infrastructure.Interfaces;

namespace MessageSilo.Infrastructure.Services
{
    public class AWSSQSConnectionGrain : MessagePlatformConnectionGrain, IAWSSQSConnectionGrain
    {
        private readonly IGrainFactory grainFactory;

        private IAmazonSQS client;

        private string queueUrl;

        public AWSSQSConnectionGrain(IGrainFactory grainFactory)
        {
            this.grainFactory = grainFactory;
        }

        public override async ValueTask DisposeAsync()
        {
            if (client is not null)
            {
                client.Dispose();
                client = null!;
            }

            await Task.CompletedTask;
        }

        public override async Task Enqueue(Domain.Entities.Message message)
        {
            await client.SendMessageAsync(queueUrl, message.Body);
        }

        public override async Task Init(ConnectionSettingsDTO settings)
        {
            this.settings = settings;

            client = new AmazonSQSClient(this.settings.AccessKey, this.settings.SecretAccessKey, Amazon.RegionEndpoint.GetBySystemName(this.settings.Region));

            queueUrl = (await client.GetQueueUrlAsync(this.settings.QueueName)).QueueUrl;

            if (this.settings.ReceiveMode != ReceiveMode.None)
                processMessageAsync();
        }

        private async Task processMessageAsync()
        {
            while (client is not null)
            {
                var response = await client.ReceiveMessageAsync(new ReceiveMessageRequest(queueUrl));

                if (response.Messages.Count == 0)
                    continue;

                var msg = response.Messages[0];

                var connection = grainFactory.GetGrain<IConnectionGrain>(this.GetPrimaryKeyString());

                var isDelivered = await connection.TransformAndSend(new Domain.Entities.Message(msg.MessageId, msg.Body));

                if (isDelivered && settings.ReceiveMode == ReceiveMode.ReceiveAndDelete)
                    await client.DeleteMessageAsync(queueUrl, msg.ReceiptHandle);
            }
        }
    }
}
