using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using RestSharp;

namespace MessageSilo.Features.Enricher
{
    public class EnricherGrain : Grain, IEnricherGrain
    {
        private readonly ILogger<EnricherGrain> logger;

        private readonly IConfiguration configuration;

        private IPersistentState<EnricherDTO> persistence { get; set; }

        private IEnricher enricher;

        public EnricherGrain([PersistentState("EnricherState")] IPersistentState<EnricherDTO> state, ILogger<EnricherGrain> logger, IConfiguration configuration)
        {
            persistence = state;
            this.logger = logger;
            this.configuration = configuration;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            if (persistence.RecordExists)
                reInit();

            await base.OnActivateAsync(cancellationToken);
        }

        public async Task Update(EnricherDTO e)
        {
            persistence.State = e;
            await persistence.WriteStateAsync();
            reInit();
        }

        public async Task<EnricherDTO> GetState()
        {
            return await Task.FromResult(persistence.State);
        }

        public async Task<Message?> Enrich(Message message)
        {
            message.Body = await enricher.TransformMessage(message.Body);
            return message;
        }

        public async Task Delete()
        {
            await this.persistence.ClearStateAsync();
        }

        private void reInit()
        {
            var settings = persistence.State;

            switch (settings.Type)
            {
                case EnricherType.Inline:
                    enricher = new InlineEnricher(settings.Function);
                    break;
                case EnricherType.API:
                    enricher = new APIEnricher(settings.Url, settings.Method ?? Method.Post);
                    break;
                case EnricherType.AI:
                    {
                        var apiKey = settings.ApiKey ?? configuration["AI_API_KEY"];
                        enricher = new AIEnricher(apiKey, settings.Command);
                        break;
                    }
            }
        }
    }
}
