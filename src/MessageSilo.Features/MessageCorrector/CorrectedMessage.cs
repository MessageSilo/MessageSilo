using MessageSilo.Shared.Models;

namespace MessageSilo.Features.MessageCorrector
{
    public class CorrectedMessage : Message
    {
        public string BodyAfterCorrection { get; set; }

        public CorrectedMessage() : base()
        {

        }

        public CorrectedMessage(Message msg) : base(msg.Id, msg.EnqueuedTime, msg.Body)
        {

        }
    }
}
