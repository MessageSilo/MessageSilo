using MessageSilo.Features.Shared.Enums;

namespace MessageSilo.Features.Shared.Models
{
    public interface IPlatform
    {
        string Id { get; }

        string DisplayName { get; }

        List<MessagePlatformType> Types { get; }
    }
}
