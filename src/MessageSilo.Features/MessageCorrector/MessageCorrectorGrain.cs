using Esprima;
using Jint;
using MessageSilo.Features.Connection;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using System;

namespace MessageSilo.Features.MessageCorrector
{
    public class MessageCorrectorGrain : Grain, IMessageCorrectorGrain
    {
        private readonly Engine engine = new Engine();

        private readonly ILogger<MessageCorrectorGrain> logger;

        private readonly IMessageRepository<CorrectedMessage> messages;

        public MessageCorrectorGrain(ILogger<MessageCorrectorGrain> logger, IMessageRepository<CorrectedMessage> messages)
        {
            this.logger = logger;
            this.messages = messages;
        }

        public async Task CorrectMessage(IConnectionGrain sourceConnection, Message msg, IConnectionGrain? targetConnection = null)
        {
            var connectionState = await sourceConnection.GetState();

            string? correctedMessageBody = correct(msg.Body, connectionState.ConnectionSettings.CorrectorFuncBody);

            if (correctedMessageBody is not null && targetConnection is not null)
                await targetConnection.Enqueue(correctedMessageBody);

            messages.Add(connectionState.ConnectionSettings.Id, new CorrectedMessage(msg)
            {
                BodyAfterCorrection = correctedMessageBody
            });

            return;
        }

        public string? correct(string message, string currectorFuncBody)
        {
            try
            {
                engine
                    .Execute($"correct = {currectorFuncBody}")
                    .Execute("serializer = (m) => { return JSON.stringify(correct(m)); }");

                var result = engine.Evaluate($"serializer({message})");

                return result.AsString();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during message correction!");
                return null;
            }
        }
    }
}
