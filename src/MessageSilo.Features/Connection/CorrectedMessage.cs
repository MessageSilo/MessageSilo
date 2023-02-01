using MessageSilo.Shared.Models;

namespace MessageSilo.Features.Connection
{
    public class CorrectedMessage : Message
    {
        public string BodyAfterCorrection { get; set; }

        public bool? IsCorrected { get; set; }

        public bool IsResent { get; set; }

        public CorrectedMessage() : base()
        {

        }

        public CorrectedMessage(Message msg) : base(msg.Id, msg.EnqueuedTime, msg.Body)
        {

        }
    }
}
