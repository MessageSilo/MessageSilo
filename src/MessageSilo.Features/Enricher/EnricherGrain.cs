using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;
using MessageSilo.Domain.Helpers;
using MessageSilo.Domain.Interfaces;
using MessageSilo.Features.EntityManager;
using MessageSilo.Shared.Models;
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

        private IEnricher enricher { get; set; }

        public EnricherGrain(ILogger<EnricherGrain> logger, IConfiguration configuration, IGrainFactory grainFactory)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.grainFactory = grainFactory;
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
                logger.LogError(ex, $"[Enricher][{name}][#{scaleSet}] Cannot enrich message [{message?.Id}]");
                throw;
            }
        }

        public async Task Init(EnricherDTO? dto = null)
        {
            var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();

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
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[Enricher][{name}][#{scaleSet}] Initialization error");
            }
        }

        private IEnricher getEnricher(EnricherDTO dto)
        {
            return dto.Type switch
            {
                EnricherType.Inline => new InlineEnricher(dto.Function),
                EnricherType.API => new APIEnricher(dto.Url, dto.Method ?? Method.Post, dto.Retry ?? new()),
                EnricherType.AI => new AIEnricher(dto.ApiKey ?? configuration["AI_API_KEY"], dto.Command),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
