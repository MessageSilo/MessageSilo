using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace MessageSilo.Features.Target
{
    public class TargetGrain : Grain, ITargetGrain
    {
        private readonly ILogger<TargetGrain> logger;

        private readonly IGrainFactory grainFactory;

        private IPersistentState<TargetDTO> persistence { get; set; }

        private ITarget target;

        private LastMessage lastMessage;

        public TargetGrain([PersistentState("TargetState")] IPersistentState<TargetDTO> state, ILogger<TargetGrain> logger, IGrainFactory grainFactory)
        {
            this.persistence = state;
            this.logger = logger;
            this.grainFactory = grainFactory;
        }

        public override async Task OnActivateAsync()
        {
            if (this.persistence.RecordExists)
                reInit();

            await base.OnActivateAsync();
        }

        public async Task Send(Message message)
        {
            try
            {
                lastMessage = new LastMessage(message);
                await target.Send(message);
                lastMessage.SetOutput(null, null);
            }
            catch (Exception ex)
            {
                lastMessage = new LastMessage();
                lastMessage.SetOutput(null, ex.Message);
                logger.LogError(ex.Message);
            }
        }

        public async Task Update(TargetDTO t)
        {
            persistence.State = t;
            await persistence.WriteStateAsync();
            reInit();
        }

        public async Task<TargetDTO> GetState()
        {
            return await Task.FromResult(persistence.State);
        }

        public async Task<LastMessage> GetLastMessage()
        {
            return await Task.FromResult(lastMessage);
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
                case TargetType.API:
                    target = new APITarget(settings.Url);
                    break;
                case TargetType.Azure_EventGrid:
                    target = new AzureEventGridTarget(settings.Endpoint, settings.AccessKey);
                    break;
            }
        }
    }
}
