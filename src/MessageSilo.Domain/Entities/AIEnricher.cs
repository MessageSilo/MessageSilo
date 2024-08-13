using MessageSilo.Domain.Interfaces;

namespace MessageSilo.Domain.Entities
{
    public class AIEnricher : IEnricher
    {
        private readonly string apiKey;

        private readonly string command;

        public AIEnricher(string apiKey, string command)
        {
            this.apiKey = apiKey;
            this.command = command;
        }

        public async Task<string> TransformMessage(string message)
        {
            return message;
        }
    }
}
