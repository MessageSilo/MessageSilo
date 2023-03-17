using MessageSilo.Shared.Enums;

namespace MessageSilo.Shared.Models
{
    public interface IMessagePlatformConnection : IAsyncDisposable
    {
        string Name { get; }

        string ConnectionString { get; }

        MessagePlatformType Type { get; }

        event EventHandler MessageReceived;

        Task InitDeadLetterCorrector();

        Task Enqueue(string msgBody);
    }
}
