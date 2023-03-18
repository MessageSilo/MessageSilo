using MessageSilo.Shared.Enums;

namespace MessageSilo.Shared.Models
{
    public interface IMessagePlatformConnection : IAsyncDisposable
    {
        string Name { get; }

        string ConnectionString { get; }

        MessagePlatformType Type { get; }

        event EventHandler MessageReceived;

        Task Init();

        Task Enqueue(string msgBody);
    }
}
