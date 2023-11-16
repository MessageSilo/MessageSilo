using MessageSilo.Features.EntityManager;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Extensions;
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
            var (userId, name, scaleSet) = this.GetPrimaryKeyString().Explode();

            var em = grainFactory.GetGrain<IEntityManagerGrain>(userId);
            var settings = await em.GetEnricherSettings(name);

            if (settings == null)
                return;

            enricher = getEnricher(settings);

            await base.OnActivateAsync(cancellationToken);
        }

        public async Task<Message?> Enrich(Message message)
        {
            message.Body = await enricher.TransformMessage(message.Body);
            return message;
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
