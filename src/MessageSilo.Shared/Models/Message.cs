namespace MessageSilo.Shared.Models
{
    public class Message
    {
        public string Id { get; set; }

        public DateTimeOffset EnqueuedTime { get; set; }

        public string Body { get; set; }

        public long SequenceNumber { get; set; }

        public Message()
        {

        }

        public Message(string id, DateTimeOffset enqueuedTime, string body, long sequenceNumber)
        {
            Id = id;
            EnqueuedTime = enqueuedTime;
            Body = body;
            SequenceNumber = sequenceNumber;
        }
    }
}
