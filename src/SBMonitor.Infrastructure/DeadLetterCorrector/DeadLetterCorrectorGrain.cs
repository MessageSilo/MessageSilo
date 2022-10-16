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
            this.messagePlatformConnection.DeadLetterMessageReceived += deadLetterMessageReceived;
            this.messagePlatformConnection.StartProcessingDeadLetterMessages();
            return Task.CompletedTask;
        }

        private void deadLetterMessageReceived(object? sender, EventArgs e)
        {
            var eventArgs = e as MessageReceivedEventArgs;

            var correctedMessage = messageCorrector.Correct(eventArgs.MessageBody, correctorFuncBody);


        }
    }
}
