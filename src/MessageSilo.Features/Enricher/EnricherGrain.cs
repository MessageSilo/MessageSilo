using MessageSilo.Features.EntityManager;
using MessageSilo.Features.Hubs;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Extensions;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace MessageSilo.Features.Enricher
{
    public class EnricherGrain : Grain, IEnricherGrain
    {
        private readonly ILogger<EnricherGrain> logger;

        private readonly IGrainFactory grainFactory;

        private readonly IConfiguration configuration;

        private readonly IHubContext<SignalHub> hubContext;

        private IEnricher enricher { get; set; }

        public EnricherGrain(ILogger<EnricherGrain> logger, IConfiguration configuration, IGrainFactory grainFactory, IHubContext<SignalHub> hubContext)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.grainFactory = grainFactory;
            this.hubContext = hubContext;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await Init();

            await base.OnActivateAsync(cancellationToken);
        }

        public async Task<Message?> Enrich(Message message)
        {
            try
            {
                message.Body = await enricher.TransformMessage(message.Body);
                return message;
            }
            catch (Exception ex)
            {
                var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();
                var msg = $"[Enricher][{name}][#{scaleSet}] Cannot enrich message [{message?.Id}] - {ex.Message}";
                logger.LogError(ex, msg);
                await hubContext.Clients.Group(userId).SendAsync("signalReceived", new Signal($"{name}#{scaleSet}", SignalType.Malfunctioned, LogLevel.Error, msg));
                throw;
            }
        }

        public async Task Init(EnricherDTO? dto = null)
        {
            var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();

            await hubContext.Clients.Group(userId).SendAsync("signalReceived", new Signal($"{name}#{scaleSet}", SignalType.Initializing, LogLevel.None, "Initialization started..."));

            try
            {
                EnricherDTO? settings = dto;

                if (settings == null)
                {
                    var em = grainFactory.GetGrain<IEntityManagerGrain>(userId);
                    settings = await em.GetEnricherSettings(name);
                }

                if (settings == null)
                    return;

                enricher = getEnricher(settings);

                await hubContext.Clients.Group(userId).SendAsync("signalReceived", new Signal($"{name}#{scaleSet}", SignalType.Active, LogLevel.Information, "Active"));
            }
            catch (Exception ex)
            {
                var msg = $"[Enricher][{name}][#{scaleSet}] Initialization error - {ex.Message}";
                logger.LogError(ex, msg);
                await hubContext.Clients.Group(userId).SendAsync("signalReceived", new Signal($"{name}#{scaleSet}", SignalType.Malfunctioned, LogLevel.Error, msg));
            }
        }

        private IEnricher getEnricher(EnricherDTO dto)
        {
            return dto.Type switch
            {
                EnricherType.Inline => new InlineEnricher(dto.Function),
                EnricherType.API => new APIEnricher(dto.Url, dto.Method ?? Method.Post),
                EnricherType.AI => new AIEnricher(dto.ApiKey ?? configuration["AI_API_KEY"], dto.Command),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
