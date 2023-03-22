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

        public MessageCorrectorGrain(ILogger<MessageCorrectorGrain> logger)
        {
            this.logger = logger;
        }

        public async Task CorrectMessage(IConnectionGrain sourceConnection, Message msg, IMessageSenderGrain? target = null)
        {
            var connectionState = await sourceConnection.GetState();

            string? correctedMessageBody = correct(msg.Body, connectionState.ConnectionSettings.CorrectorFuncBody);

            if (correctedMessageBody is not null && target is not null)
                await target.Send(correctedMessageBody);

            return;
        }

        public string? correct(string message, string correctorFuncBody)
        {
            try
            {
                if (string.IsNullOrEmpty(correctorFuncBody))
                    return message;

                engine
                    .Execute($"correct = {correctorFuncBody}")
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
