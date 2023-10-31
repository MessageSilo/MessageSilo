namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class Message
    {
        [Id(0)]
        public string Id { get; set; }

        [Id(1)]
        public string Body { get; set; }

        public Message()
        {

        }

        public Message(string id, string body)
        {
            Id = id;
            Body = body;
        }

        public Message GetCopy()
        {
            return new Message
            {
                Id = Id,
                Body = Body
            };
        }
    }
}
