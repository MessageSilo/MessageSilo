﻿using Amazon.SQS;
using Amazon.SQS.Model;
using MessageSilo.Shared.Models;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Features.AWS
{
    public class AWSSQSConnection : MessagePlatformConnection
    {
        private readonly ILogger logger;

        private IAmazonSQS client;

        private string queueUrl;

        public string QueueName { get; }

        public string Region { get; }

        public string AccessKey { get; }

        public string SecretAccessKey { get; }

        public AWSSQSConnection(string queueName, string region, string accessKey, string secretAccessKey, bool autoAck, ILogger logger)
        {
            QueueName = queueName;
            Region = region;
            AccessKey = accessKey;
            SecretAccessKey = secretAccessKey;
            AutoAck = autoAck;
            this.logger = logger;
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

        public override async Task Enqueue(string msgBody)
        {
            await client.SendMessageAsync(queueUrl, msgBody);
        }

        public override async Task Init()
        {
            client = new AmazonSQSClient(AccessKey, SecretAccessKey, Amazon.RegionEndpoint.GetBySystemName(Region));

            queueUrl = (await client.GetQueueUrlAsync(QueueName)).QueueUrl;

            processMessageAsync();

            await Task.CompletedTask;
        }

        private async Task processMessageAsync()
        {
            while (client is not null)
            {
                var response = await client.ReceiveMessageAsync(new ReceiveMessageRequest(queueUrl));

                if (response.Messages.Count == 0)
                    continue;

                var msg = response.Messages[0];

                OnMessageReceived(new MessageReceivedEventArgs(new Shared.Models.Message(msg.MessageId, msg.Body)));

                if (AutoAck)
                    await client.DeleteMessageAsync(queueUrl, msg.ReceiptHandle);
            }
        }
    }
}
