﻿using MessageSilo.Features.Connection;
using MessageSilo.Shared.Models;
using Orleans;

namespace MessageSilo.Features.MessageCorrector
{
    public interface IMessageCorrectorGrain : IGrainWithGuidKey
    {
        Task CorrectMessages(IConnectionGrain sourceConnection, List<Message> msgs, IConnectionGrain? targetConnection = null);
    }
}
