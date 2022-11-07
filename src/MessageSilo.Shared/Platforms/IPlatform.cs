using MessageSilo.Shared.Enums;

namespace MessageSilo.Shared.Platforms
{
    public interface IPlatform
    {
        string Id { get; }

        string DisplayName { get; }

        List<MessagePlatformType> Types { get; }
    }
}
