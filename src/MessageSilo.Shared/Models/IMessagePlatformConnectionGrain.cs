namespace MessageSilo.Shared.Models
{
    public interface IMessagePlatformConnectionGrain : IAsyncDisposable, IGrainWithStringKey
    {
        Task Init(ConnectionSettingsDTO settings);

        Task Enqueue(Message message);
    }
}
