using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using SBMonitor.Core.DeadLetterCorrector;
using SBMonitor.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Infrastructure.DeadLetterCorrector
{
    public class DeadLetterCorrectorGrain : Grain, IDeadLetterCorrectorGrain
    {
        private IMessageCorrector messageCorrector;

        private IMessagePlatformConnection messagePlatformConnection;

        private string correctorFuncBody;

        public DeadLetterCorrectorGrain(IMessageCorrector messageCorrector)
        {
            this.messageCorrector = messageCorrector;
        }

        public Task Init(IMessagePlatformConnection messagePlatformConnection, string correctorFuncBody)
        {
            this.correctorFuncBody = correctorFuncBody;
            this.messagePlatformConnection = messagePlatformConnection;
            this.messagePlatformConnection.InitDeadLetterCorrector();
            RegisterTimer(processMessagesAsync, null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        private async Task processMessagesAsync(object state)
        {
            var msgs = await messagePlatformConnection.GetDeadLetterMessagesAsync();

            foreach (var msg in msgs)
            {
                var correctedMsg = messageCorrector.Correct(msg, correctorFuncBody);
            }
        }
    }
}
