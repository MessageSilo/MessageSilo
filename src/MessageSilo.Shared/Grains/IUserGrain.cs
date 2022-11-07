using MessageSilo.Shared.Models;
using Orleans;

namespace MessageSilo.Shared.Grains
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task AddDeadLetterCorrector(ConnectionSettingsDTO setting);

        Task InitDeadLetterCorrectors();
    }
}
