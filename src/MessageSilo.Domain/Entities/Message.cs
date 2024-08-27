namespace MessageSilo.Domain.Entities
{
    public class Message
    {
        public string Id { get; set; }

        public string Body { get; set; }

        public Message(string id, string body)
        {
            Id = id;
            Body = body;
        }
    }
}
