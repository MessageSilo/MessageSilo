namespace MessageSilo.Shared.Models
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public Message Message { get; private set; }

        public MessageReceivedEventArgs(Message message)
        {
            Message = message;
        }
    }
}
