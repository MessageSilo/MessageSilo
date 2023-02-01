using Jint;
using Microsoft.Extensions.Logging;
using System;

namespace MessageSilo.Features.Connection
{
    public class MessageCorrector : IMessageCorrector
    {
        private readonly Engine engine = new Engine();

        private readonly ILogger<MessageCorrector> logger;

        public MessageCorrector(ILogger<MessageCorrector> logger)
        {
            this.logger = logger;
        }

        public string Correct(string message, string currectorFuncBody)
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
