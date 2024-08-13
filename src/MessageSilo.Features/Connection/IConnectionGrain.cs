using MessageSilo.Domain.Entities;
using MessageSilo.Shared.Models;
using Orleans.Concurrency;

namespace MessageSilo.Features.Connection
{
    public interface IConnectionGrain : IMessageSenderGrain
    {
        Task<bool> TransformAndSend(Message message);

        [OneWay]
        Task Delete();

        Task Health();

        [OneWay]
        Task Init(bool force = false);
    }
}
