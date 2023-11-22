using MessageSilo.Shared.Models;
using Orleans.Concurrency;

namespace MessageSilo.Features.Target
{
    public interface ITargetGrain : IMessageSenderGrain
    {
        [OneWay]
        Task Init(TargetDTO? dto = null);
    }
}
