using MessageSilo.Domain.Entities;

namespace MessageSilo.Domain.Interfaces
{
    public interface ITarget
    {
        Task Send(Message message);
    }
}
