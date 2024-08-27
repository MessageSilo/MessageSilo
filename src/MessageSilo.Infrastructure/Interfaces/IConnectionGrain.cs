using MessageSilo.Domain.Entities;
using Orleans.Concurrency;

namespace MessageSilo.Infrastructure.Interfaces
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
