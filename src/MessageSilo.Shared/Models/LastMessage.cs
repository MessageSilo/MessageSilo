using MessageSilo.Shared.Serialization;

namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class LastMessage
    {
        [Id(0)]
        public Message Input { get; set; }

        [Id(1)]
        public Message? Output { get; set; }

        [Id(2)]
        public string? Error { get; set; }

        public LastMessage(Message input)
        {
            Input = input.GetCopy();
        }

        public LastMessage()
        {
        }

        public void SetOutput(Message? output, string? error)
        {
            Output = output?.GetCopy();
            Error = error;
        }

        public override string ToString()
        {
            return YamlConverter.Serialize(this);
        }
    }
}
