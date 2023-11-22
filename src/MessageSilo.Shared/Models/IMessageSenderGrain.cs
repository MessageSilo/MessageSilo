using Orleans.Concurrency;

namespace MessageSilo.Shared.Models
{
    public interface IMessageSenderGrain : IGrainWithStringKey
    {
        Task Send(Message message);
    }
}
