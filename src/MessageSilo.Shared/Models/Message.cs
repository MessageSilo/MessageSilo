using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace MessageSilo.Shared.Models
{
    public class Message
    {
        public string Id { get; set; }

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
