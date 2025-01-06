using MessageSilo.Domain.Interfaces;

namespace MessageSilo.Domain.Entities
{
    public class AIEnricher : IEnricher
    {
        private const string PROMPT_TEMPLATE = @"
        ## Task
        {0}

        ### Input
        You will receive a JSON object or invalid JSON.

        ### Output
        Ensure the output is strictly formatted as valid JSON with no additional text or comments.
        ";

        private readonly IAIService aiService;

        private readonly string prompt;

        public AIEnricher(IAIService aiService, string command)
        {
            this.aiService = aiService;
            this.prompt = string.Format(PROMPT_TEMPLATE, command);
        }

        public async Task<string> TransformMessage(string message)
        {
            return await aiService.Chat(prompt, message);
        }
    }
}
