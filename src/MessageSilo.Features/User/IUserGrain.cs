using MessageSilo.Features.Shared.Models;
using Orleans;

namespace MessageSilo.Features.User
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task AddDeadLetterCorrector(ConnectionSettingsDTO setting);

        Task InitDeadLetterCorrectors();
    }
}
