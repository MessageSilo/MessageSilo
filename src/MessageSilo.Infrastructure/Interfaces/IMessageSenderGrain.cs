using MessageSilo.Domain.Entities;

namespace MessageSilo.Infrastructure.Interfaces
{
    public interface IMessageSenderGrain : IGrainWithStringKey
    {
        Task Send(Message message);
    }
}
