using MessageSilo.Features.Shared.Models;
using Orleans;

namespace MessageSilo.Features.DeadLetterCorrector
{
    public interface IDeadLetterCorrectorGrain : IGrainWithStringKey
    {
        Task Init(IMessagePlatformConnection messagePlatformConnection, string correctorFuncBody);

        Task<List<CorrectedMessage>> GetCorrectedMessages();
    }
}
