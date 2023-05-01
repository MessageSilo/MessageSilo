using Jint;
using MessageSilo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Features.Enricher
{
    public class InlineEnricher : IEnricher
    {
        private readonly Engine engine = new Engine();

        private readonly string function;

        public InlineEnricher(string function)
        {
            this.function = function;
        }

        public async Task<string> TransformMessage(string message)
        {
            if (string.IsNullOrEmpty(function))
                return message;

            engine
                .Execute($"correct = {function}")
                .Execute("serializer = (m) => { return JSON.stringify(correct(m)); }");

            var result = engine.Evaluate($"serializer({message})");

            return await Task.FromResult(result.AsString());
        }
    }
}
