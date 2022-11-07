using MessageSilo.Features.Azure;
using MessageSilo.Features.DeadLetterCorrector;
using MessageSilo.Features.Shared.Enums;
using MessageSilo.Features.Shared.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace MessageSilo.Features.User
{
    public class UserGrain : Grain, IUserGrain
    {
        protected ILogger<UserGrain> logger;

        protected IGrainFactory grainFactory;

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
            deadLetterCorrectorSettings.State.Add(setting);
            await deadLetterCorrectorSettings.WriteStateAsync();
            return;
        }

        public Task InitDeadLetterCorrectors()
        {
            foreach (var setting in deadLetterCorrectorSettings.State)
            {
                var deadLetterCorrector = grainFactory.GetGrain<IDeadLetterCorrectorGrain>($"{this.GetPrimaryKeyString()}_{setting.Name}");

                switch (setting.Type)
                {
                    case MessagePlatformType.Azure_Queue:
                        deadLetterCorrector.Init(new AzureServiceBusConnection(setting.ConnectionString, setting.QueueName), setting.CorrectorFuncBody);
                        break;
                    case MessagePlatformType.Azure_Topic:
                        deadLetterCorrector.Init(new AzureServiceBusConnection(setting.ConnectionString, setting.TopicName, setting.SubscriptionName), setting.CorrectorFuncBody);
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
