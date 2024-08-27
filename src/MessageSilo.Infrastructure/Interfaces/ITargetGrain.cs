using MessageSilo.Application.DTOs;
using Orleans.Concurrency;

namespace MessageSilo.Infrastructure.Interfaces
{
    public interface ITargetGrain : IMessageSenderGrain
    {
        [OneWay]
        Task Init(TargetDTO? dto = null);
    }
}
