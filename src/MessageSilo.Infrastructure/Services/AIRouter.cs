using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;
using MessageSilo.Domain.Interfaces;
using MessageSilo.Infrastructure.Interfaces;
using Newtonsoft.Json;

namespace MessageSilo.Infrastructure.Services
{
    public class AIRouter : ITarget
    {
        private const string PROMPT_TEMPLATE = @"
        ## Task
        Based on the given rules, determine which target(s) should process the input JSON.

        ### Rules
        {0}

        ### Input
        You will receive a JSON object or invalid JSON.

        ### Output
        Return a JSON array of target names that match the rules, for example: '{{ ""targets"": [""target1""] }}'. If no rules match, return an empty array '{{ ""targets"": [] }}'.

        ### Notes
        - The output must always be a valid JSON array.
        - Multiple rules can match simultaneously, so include all applicable targets.
        ";

        private readonly IEntityManagerGrain entityManager;

        private readonly IGrainFactory grainFactory;

        private readonly IAIService aiService;

        private readonly string prompt;

        public AIRouter(string userId, IAIService aiService, IEnumerable<AIRouterRule> rules, IGrainFactory grainFactory)
        {
            this.aiService = aiService;
            this.grainFactory = grainFactory;

            var rulesPropmpt = string.Join(Environment.NewLine, rules.Select((rule, index) =>
            $"- {rule.TargetName}: {rule.Condition}"));

            this.prompt = string.Format(PROMPT_TEMPLATE, rulesPropmpt);

            this.entityManager = this.grainFactory.GetGrain<IEntityManagerGrain>(userId);
        }

        public async Task Send(Message message)
        {
            var targetNames = await getTargetNames(message.Body);

            var entities = await entityManager.List();

            var targets = entities.Where(p => targetNames.Contains(p.Name));

            foreach (var target in targets)
            {
                IMessageSenderGrain sender = getTarget(target);
                await sender.Send(message);
            }
        }

        private async Task<IEnumerable<string>> getTargetNames(string message)
        {
            var aiResponse = await aiService.Chat(prompt, message);

            var aiRouterResponse = JsonConvert.DeserializeObject<AIRouterResponse>(aiResponse);

            return aiRouterResponse.Targets;
        }

        private IMessageSenderGrain getTarget(Entity targetEntity)
        {
            var targetId = $"{targetEntity.Id}#{1}";

            return targetEntity.Kind switch
            {
                EntityKind.Connection => grainFactory.GetGrain<IConnectionGrain>(targetId),
                EntityKind.Target => grainFactory.GetGrain<ITargetGrain>(targetId),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
