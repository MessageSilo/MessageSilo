using InfluxDB.Client.Api.Domain;
using MessageSilo.Features.AWS;
using MessageSilo.Features.Azure;
using MessageSilo.Features.Connection;
using MessageSilo.Features.RabbitMQ;
using MessageSilo.Features.Target;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Features.Enricher
{
    public class EnricherGrain : Grain, IEnricherGrain
    {
        private readonly ILogger<EnricherGrain> logger;

        private IPersistentState<EnricherDTO> persistence { get; set; }

        private IEnricher enricher;

        private LastMessage lastMessage;

        public EnricherGrain([PersistentState("EnricherState")] IPersistentState<EnricherDTO> state, ILogger<EnricherGrain> logger)
        {
            this.persistence = state;
            this.logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            if (this.persistence.RecordExists)
                reInit();

            await base.OnActivateAsync();
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

        public async Task<LastMessage> GetLastMessage()
        {
            return await Task.FromResult(lastMessage);
        }

        public async Task<Message?> Enrich(Message message)
        {
            lastMessage = new LastMessage(message);

            try
            {
                message.Body = await enricher.TransformMessage(message.Body);
                lastMessage.SetOutput(message, null);
                return message;

            }
            catch (Exception ex)
            {
                lastMessage.SetOutput(null, ex.Message);
                return null;
            }
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
            }
        }

        public async Task Delete()
        {
            await this.persistence.ClearStateAsync();
        }
    }
}
