using MessageSilo.Shared.Models;

namespace MessageSilo.Features.Target
{
    public interface ITargetGrain : IEntityGrain<TargetDTO, TargetDTO>, IMessageSenderGrain
    {
    }
}
