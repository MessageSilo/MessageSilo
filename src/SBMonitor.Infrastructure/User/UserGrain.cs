using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using Orleans.Runtime;
using SBMonitor.Core.Enums;
using SBMonitor.Core.Shared;
using SBMonitor.Infrastructure.DeadLetterCorrector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Infrastructure.User
{
    public class UserGrain : Grain, IUserGrain
    {
        protected ILogger<UserGrain> logger;

        protected IGrainFactory grainFactory;

        protected IPersistentState<List<string>> deadLetterCorrectorSettings { get; set; }

        public UserGrain(
            ILogger<UserGrain> logger,
            [PersistentState("DeadLetterCorrectorSettings")] IPersistentState<List<string>> deadLetterCorrectorSettings,
            IGrainFactory grainFactory) : base()
        {
            this.logger = logger;
            this.deadLetterCorrectorSettings = deadLetterCorrectorSettings;
            this.grainFactory = grainFactory;
        }

        public async Task AddDeadLetterCorrector(string settingString)
        {
            deadLetterCorrectorSettings.State.Add(settingString);
            await deadLetterCorrectorSettings.WriteStateAsync();
            return;
        }

        public Task InitDeadLetterCorrectors()
        {
            foreach (var settingString in deadLetterCorrectorSettings.State)
            {
                var setting = JsonConvert.DeserializeObject<dynamic>(settingString);

                var deadLetterCorrector = grainFactory.GetGrain<IDeadLetterCorrectorGrain>($"a@b.com_{setting.name}");

                switch (setting.type)
                {
                    case MessagePlatformType.Azure_Queue:
                        deadLetterCorrector.Init(new AzureServiceBusConnection(setting.queueName), setting.correctorFuncBody);
                        break;
                    case MessagePlatformType.Azure_Topic:
                        deadLetterCorrector.Init(new AzureServiceBusConnection(setting.topicName, setting.subscriptionName), setting.correctorFuncBody);
                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
