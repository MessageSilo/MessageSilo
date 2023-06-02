using MessageSilo.Shared.Enums;

namespace MessageSilo.Shared.Models
{
    public abstract class MessagePlatformConnection : IMessagePlatformConnection
    {
        public string Name { get; protected set; }

        public string ConnectionString { get; protected set; }

        public MessagePlatformType Type { get; protected set; }

        public ReceiveMode ReceiveMode { get; protected set; }

        public event EventHandler MessageReceived;

        protected bool autoAck => ReceiveMode == ReceiveMode.ReceiveAndDelete;

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        public abstract Task Init();

        public abstract Task Enqueue(Message message);

        public abstract ValueTask DisposeAsync();
    }
}
