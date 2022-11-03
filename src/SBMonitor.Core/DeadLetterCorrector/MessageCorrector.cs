using Jint;

namespace SBMonitor.Core.DeadLetterCorrector
{
    public class MessageCorrector : IMessageCorrector
    {
        private readonly Engine engine = new Engine();

        public string Correct(string message, string currectorFuncBody)
        {
            engine
                .Execute($"correct = {currectorFuncBody}")
                .Execute("serializer = (m) => { return JSON.stringify(correct(m)); }");

            var result = engine.Evaluate($"serializer({message})");

            return result.AsString();
        }
    }
}
