using MessageSilo.Shared.Models;

namespace MessageSilo.Features.Target
{
    public interface ITarget
    {
        Task Send(Message message);
    }
}
