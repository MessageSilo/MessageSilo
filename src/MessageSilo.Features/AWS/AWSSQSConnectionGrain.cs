using Amazon.SQS;
using Amazon.SQS.Model;
using MessageSilo.Features.Connection;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;

namespace MessageSilo.Features.AWS
{
    public class AWSSQSConnectionGrain : MessagePlatformConnectionGrain, IAWSSQSConnectionGrain
    {
        private readonly ILogger<AWSSQSConnectionGrain> logger;

        private readonly IGrainFactory grainFactory;

        private IAmazonSQS client;

        private string queueUrl;

        public AWSSQSConnectionGrain(ILogger<AWSSQSConnectionGrain> logger, IGrainFactory grainFactory)
        {
            this.logger = logger;
            this.grainFactory = grainFactory;
        }

        public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken token)
        {
            var grain = grainFactory.GetGrain<IAWSSQSConnectionGrain>(this.GetPrimaryKeyString());

           grain.GetPrimaryKeyString();

            return Task.CompletedTask;
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

        public override async Task Enqueue(Shared.Models.Message message)
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

                await connection.TransformAndSend(new Shared.Models.Message(msg.MessageId, msg.Body));

                if (this.settings.ReceiveMode == ReceiveMode.ReceiveAndDelete)
                    await client.DeleteMessageAsync(queueUrl, msg.ReceiptHandle);
            }
        }
    }
}
