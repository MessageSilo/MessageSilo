using MessageSilo.Features.Connection;
using MessageSilo.Shared.Models;
using Orleans;

namespace MessageSilo.Features.MessageCorrector
{
    public interface IMessageCorrectorGrain : IGrainWithStringKey
    {
        Task CorrectMessage(IConnectionGrain sourceConnection, Message msg, IConnectionGrain? targetConnection = null);
    }
}
