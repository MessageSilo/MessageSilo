namespace MessageSilo.Application.DTOs
{
    public class MessageDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public required string Body { get; set; }
    }
}
