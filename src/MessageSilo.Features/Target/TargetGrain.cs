using Amazon.Util;
using MessageSilo.Features.Connection;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Features.Target
{
    public class TargetGrain : Grain, ITargetGrain
    {
        private readonly ILogger<TargetGrain> logger;

        private readonly IGrainFactory grainFactory;

        private IPersistentState<TargetDTO> persistence { get; set; }

        private IRestClient client = new RestClient();

        public TargetGrain([PersistentState("TargetState")] IPersistentState<TargetDTO> state, ILogger<TargetGrain> logger, IGrainFactory grainFactory)
        {
            this.persistence = state;
            this.logger = logger;
            this.grainFactory = grainFactory;
        }

        public async Task Send(Message message)
        {
            var request = new RestRequest(persistence.State.Url, Method.Post);
            request.AddBody(message.Body, contentType: ContentType.Json);

            var response = await client.ExecutePostAsync(request);

            if (!response.IsSuccessful)
            {
                logger.LogError(response.ErrorMessage);
            }
        }

        public async Task Update(TargetDTO t)
        {
            persistence.State = t;
            await persistence.WriteStateAsync();
        }

        public async Task<TargetDTO> GetState()
        {
            return await Task.FromResult(persistence.State);
        }

        public async Task Delete()
        {
            await this.persistence.ClearStateAsync();
        }
    }
}
