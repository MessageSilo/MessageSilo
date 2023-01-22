using MessageSilo.Shared.Enums;

namespace MessageSilo.Shared.Platforms
{
    public interface IPlatform
    {
        string Id { get; }

        string Title { get; }

        List<MessagePlatformType> Types { get; }
    }
}
