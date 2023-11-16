using MessageSilo.Shared.Models;
using Orleans.Concurrency;

namespace MessageSilo.Features.Connection
{
    public interface IConnectionGrain : IMessageSenderGrain
    {
        [OneWay]
        Task TransformAndSend(Message message);

        [OneWay]
        Task Delete();

        Task Health();

        [OneWay]
        Task Init();
    }
}
