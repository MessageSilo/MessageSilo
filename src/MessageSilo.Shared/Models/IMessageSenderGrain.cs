using Orleans.Concurrency;

namespace MessageSilo.Shared.Models
{
    public interface IMessageSenderGrain : IGrainWithStringKey
    {
        [OneWay]
        Task Send(Message message);
    }
}
