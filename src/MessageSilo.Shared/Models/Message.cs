namespace MessageSilo.Shared.Models
{
    public class Message
    {
        public string Id { get; set; }

        public DateTimeOffset EnqueuedTime { get; set; }

        public string Body { get; set; }

        public Message(string id, DateTimeOffset enqueuedTime, string body)
        {
            Id = id;
            EnqueuedTime = enqueuedTime;
            Body = body;
        }
    }
}
