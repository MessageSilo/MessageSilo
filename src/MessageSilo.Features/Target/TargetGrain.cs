using MessageSilo.Features.EntityManager;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Extensions;
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
            var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();

            var em = grainFactory.GetGrain<IEntityManagerGrain>(userId);
            var settings = await em.GetTargetSettings(name);

            if (settings == null)
                return;

            target = getTarget(settings);

            await base.OnActivateAsync(cancellationToken);
        }

        public async Task Send(Message message)
        {
            await target.Send(message);
        }

        private ITarget getTarget(TargetDTO dto)
        {
            return dto.Type switch
            {
                TargetType.API => new APITarget(dto.Url),
                TargetType.Azure_EventGrid => new AzureEventGridTarget(dto.Endpoint, dto.AccessKey),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
