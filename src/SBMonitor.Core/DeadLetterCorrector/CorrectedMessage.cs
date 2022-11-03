using SBMonitor.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.DeadLetterCorrector
{
    public class CorrectedMessage : Message
    {
        public string BodyAfterCorrection { get; set; }

        public bool? IsCorrected { get; set; }

        public bool IsResent { get; set; }

        public CorrectedMessage(Message msg) : base(msg.Id, msg.EnqueuedTime, msg.Body)
        {

        }
    }
}
