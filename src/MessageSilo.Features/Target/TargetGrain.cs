using MessageSilo.Features.EntityManager;
using MessageSilo.Features.Hubs;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Extensions;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MessageSilo.Features.Target
{
    public class TargetGrain : Grain, ITargetGrain
    {
        private readonly ILogger<TargetGrain> logger;

        private readonly IGrainFactory grainFactory;

        private readonly IHubContext<SignalHub> hubContext;

        private ITarget target;

        public TargetGrain(ILogger<TargetGrain> logger, IGrainFactory grainFactory, IHubContext<SignalHub> hubContext)
        {
            this.logger = logger;
            this.grainFactory = grainFactory;
            this.hubContext = hubContext;
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
                var msg = $"[Target][{name}][#{scaleSet}] Cannot send message [{message?.Id}] - {ex.Message}";
                logger.LogError(ex, msg);
                await hubContext.Clients.Group(userId).SendAsync("signalReceived", new Signal($"{name}#{scaleSet}", SignalType.Malfunctioned, LogLevel.Error, msg));
                throw;
            }
        }

        public async Task Init(TargetDTO? dto = null)
        {
            var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();

            await hubContext.Clients.Group(userId).SendAsync("signalReceived", new Signal($"{name}#{scaleSet}", SignalType.Initializing, LogLevel.None, "Initialization started..."));

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

                await hubContext.Clients.Group(userId).SendAsync("signalReceived", new Signal($"{name}#{scaleSet}", SignalType.Active, LogLevel.Information, "Active"));
            }
            catch (Exception ex)
            {
                var msg = $"[Target][{name}][#{scaleSet}] Initialization error - {ex.Message}";
                logger.LogError(ex, msg);
                await hubContext.Clients.Group(userId).SendAsync("signalReceived", new Signal($"{name}#{scaleSet}", SignalType.Malfunctioned, LogLevel.Error, msg));
            }
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
