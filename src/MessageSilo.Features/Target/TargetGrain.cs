using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;
using MessageSilo.Domain.Helpers;
using MessageSilo.Domain.Interfaces;
using MessageSilo.Features.EntityManager;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;

namespace MessageSilo.Features.Target
{
    public class TargetGrain : Grain, ITargetGrain
    {
        private readonly ILogger<TargetGrain> logger;

        private readonly IGrainFactory grainFactory;

        private ITarget target;

        public TargetGrain(ILogger<TargetGrain> logger, IGrainFactory grainFactory)
        {
            this.logger = logger;
            this.grainFactory = grainFactory;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await Init();

            await base.OnActivateAsync(cancellationToken);
        }

        public async Task Send(Message message)
        {
            try
            {
                await target.Send(message);
            }
            catch (Exception ex)
            {
                var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();
                logger.LogError(ex, $"[Target][{name}][#{scaleSet}] Cannot send message [{message?.Id}]");
                throw;
            }
        }

        public async Task Init(TargetDTO? dto = null)
        {
            var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();

            try
            {
                TargetDTO? settings = dto;

                if (settings == null)
                {
                    var em = grainFactory.GetGrain<IEntityManagerGrain>(userId);
                    settings = await em.GetTargetSettings(name);
                }

                if (settings == null)
                    return;

                target = getTarget(settings);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[Target][{name}][#{scaleSet}] Initialization error");
            }
        }

        private ITarget getTarget(TargetDTO dto)
        {
            return dto.Type switch
            {
                TargetType.API => new APITarget(dto.Url, dto.Retry ?? new()),
                TargetType.Azure_EventGrid => new AzureEventGridTarget(dto.Endpoint, dto.AccessKey),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
