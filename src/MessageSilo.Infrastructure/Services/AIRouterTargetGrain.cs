using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Helpers;
using MessageSilo.Domain.Interfaces;
using MessageSilo.Infrastructure.Interfaces;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;

namespace MessageSilo.Infrastructure.Services
{
    public class AIRouterTargetGrain : TargetGrain
    {
        private IAIRouter aiRouter;

        private readonly IConfiguration configuration;

        private readonly IEntityManagerGrain entityManager;

        public AIRouterTargetGrain(ILogger<AIRouterTargetGrain> logger, IConfiguration configuration, IGrainFactory grainFactory) : base(logger, grainFactory)
        {
            var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();
            this.configuration = configuration;
            this.entityManager = grainFactory.GetGrain<IEntityManagerGrain>(userId);
        }

        public override async Task Init(TargetDTO? dto = null)
        {
            var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();

            try
            {
                TargetDTO? settings = dto;

                if (settings == null)
                {
                    settings = await entityManager.GetTargetSettings(name);
                }

                if (settings == null)
                    return;

                aiRouter = new AIRouter(new AIService(
                        dto.ApiKey ?? configuration["AI_API_KEY"],
                        dto.Model ?? configuration["AI_MODEL"]
                        ),
                        dto.Rules
                    );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[Target][{name}][#{scaleSet}] Initialization error");
            }
        }

        public override async Task Send(Message message)
        {
            try
            {
                var targetNames = await aiRouter.GetTargetNames(message.Body);
                var entities = await entityManager.List();

                var targets = entities.Where(p => targetNames.Contains(p.Name));

                foreach (var target in targets)
                {
                    //TODO
                }
            }
            catch (Exception ex)
            {
                var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();
                logger.LogError(ex, $"[Target][{name}][#{scaleSet}] Cannot send message [{message?.Id}]");
                throw;
            }
        }
    }
}
