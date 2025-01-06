using MessageSilo.Domain.Interfaces;
using Newtonsoft.Json;

namespace MessageSilo.Domain.Entities
{
    public class AIRouter : IAIRouter
    {
        private const string PROMPT_TEMPLATE = @"
        ## Task
        Based on the given rules, determine which target(s) should process the input JSON.

        ### Rules
        {0}

        ### Input
        You will receive a JSON object or invalid JSON.

        ### Output
        Return a JSON array of target names that match the rules. If no rules match, return an empty array `[]`.

        ### Notes
        - The output must always be a valid JSON array.
        - Multiple rules can match simultaneously, so include all applicable targets.
        ";

        private readonly IAIService aiService;

        private readonly string prompt;

        public AIRouter(IAIService aiService, IEnumerable<AIRouterRule> rules)
        {
            this.aiService = aiService;

            var rulesPropmpt = string.Join(Environment.NewLine, rules.Select((rule, index) =>
            $"- **{rule.TargetName}**: {rule.Condition}"));

            this.prompt = string.Format(PROMPT_TEMPLATE, rulesPropmpt);
        }

        public async Task<IEnumerable<string>> GetTargetNames(string message)
        {
            var aiResponse =  await aiService.Chat(prompt, message);

            var targetNames = JsonConvert.DeserializeObject<IEnumerable<string>>(aiResponse);

            return targetNames;
        }
    }
}
