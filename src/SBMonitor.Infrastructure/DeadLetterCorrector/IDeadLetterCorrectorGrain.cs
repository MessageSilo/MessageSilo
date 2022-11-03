using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using SBMonitor.Core.DeadLetterCorrector;
using SBMonitor.Core.Shared;

namespace SBMonitor.Infrastructure.DeadLetterCorrector
{
    public interface IDeadLetterCorrectorGrain : IGrainWithStringKey
    {
        Task Init(IMessagePlatformConnection messagePlatformConnection, string correctorFuncBody);

        Task<List<CorrectedMessage>> GetCorrectedMessages();
    }
}
