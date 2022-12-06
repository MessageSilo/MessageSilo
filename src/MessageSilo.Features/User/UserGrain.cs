using MessageSilo.Features.Azure;
using MessageSilo.Features.DeadLetterCorrector;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace MessageSilo.Features.User
{
    public class UserGrain : Grain, IUserGrain
    {
        private readonly ILogger<UserGrain> logger;
        private readonly IGrainFactory grainFactory;

        protected IPersistentState<List<ConnectionSettingsDTO>> deadLetterCorrectorSettings { get; set; }

        public UserGrain(
            ILogger<UserGrain> logger,
            [PersistentState("DeadLetterCorrectorSettings")] IPersistentState<List<ConnectionSettingsDTO>> deadLetterCorrectorSettings,
            IGrainFactory grainFactory) : base()
        {
            this.logger = logger;
            this.deadLetterCorrectorSettings = deadLetterCorrectorSettings;
            this.grainFactory = grainFactory;
        }

        public async Task AddDeadLetterCorrector(ConnectionSettingsDTO setting)
        {
            var existing = deadLetterCorrectorSettings.State.FirstOrDefault(p => p.Id == setting.Id);

            if (existing is not null)
            {
                //TODO: update
            }
            else
                deadLetterCorrectorSettings.State.Add(setting);

            await deadLetterCorrectorSettings.WriteStateAsync();

            var deadLetterCorrector = grainFactory.GetGrain<IDeadLetterCorrectorGrain>(setting.Id);
            await deadLetterCorrector.Update(setting);

            return;
        }

        public Task InitDeadLetterCorrectors()
        {
            foreach (var setting in deadLetterCorrectorSettings.State)
            {
                var deadLetterCorrector = grainFactory.GetGrain<IDeadLetterCorrectorGrain>(setting.Id);
            }

            return Task.CompletedTask;
        }

        public Task<List<ConnectionSettingsDTO>> GetDeadLetterCorrectors()
        {
            return Task.FromResult(deadLetterCorrectorSettings.State);
        }
    }
}
