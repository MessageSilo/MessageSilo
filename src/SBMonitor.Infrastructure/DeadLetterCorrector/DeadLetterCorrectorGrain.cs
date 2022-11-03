﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using SBMonitor.Core.DeadLetterCorrector;
using SBMonitor.Core.Models;
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

        private IPersistentState<List<CorrectedMessage>> correctedMessages;

        private SlidingBuffer<CorrectedMessage> buffer = new SlidingBuffer<CorrectedMessage>(1000);

        private string correctorFuncBody;

        public DeadLetterCorrectorGrain(IMessageCorrector messageCorrector, [PersistentState("correctedMessages")] IPersistentState<List<CorrectedMessage>> correctedMessages)
        {
            this.messageCorrector = messageCorrector;
            this.correctedMessages = correctedMessages;
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

            if (msgs.Count() == 0)
                return;

            foreach (var msg in msgs)
            {
                if (correctedMessages.State.Any(p => p.Id == msg.Id))
                    continue;

                string? correctedMessageBody = null;

                try
                {
                    correctedMessageBody = messageCorrector.Correct(msg.Body, correctorFuncBody);
                }
                catch(Exception ex)
                {
                    ex.ToString();
                }

                buffer.Add(new CorrectedMessage(msg)
                {
                    BodyAfterCorrection = correctedMessageBody!,
                    IsCorrected = correctedMessageBody != null,
                    IsResent = false,
                });
            }

            correctedMessages.State = buffer.ToList();
            await correctedMessages.WriteStateAsync();
        }

        public Task<List<CorrectedMessage>> GetCorrectedMessages()
        {
            return Task.FromResult(correctedMessages.State.ToList());
        }
    }
}
