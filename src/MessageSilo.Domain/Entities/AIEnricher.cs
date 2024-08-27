using MessageSilo.Domain.Interfaces;

namespace MessageSilo.Domain.Entities
{
    public class AIEnricher : IEnricher
    {
        private readonly IAIService aIService;

        private readonly string command;

        public AIEnricher(IAIService aIService, string command)
        {
            this.aIService = aIService;
            this.command = command;
        }

        public async Task<string> TransformMessage(string message)
        {
            return await aIService.Chat(command, message);
        }
    }
}
