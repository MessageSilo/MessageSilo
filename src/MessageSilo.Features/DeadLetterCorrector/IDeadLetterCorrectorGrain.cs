using MessageSilo.Shared.Models;
using Orleans;

namespace MessageSilo.Features.DeadLetterCorrector
{
    public interface IDeadLetterCorrectorGrain : IGrainWithGuidKey
    {
        Task Update(ConnectionSettingsDTO s);

        Task Delete();
    }
}
