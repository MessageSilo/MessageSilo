using MessageSilo.Shared.Models;
using Orleans;

namespace MessageSilo.Features.Connection
{
    public interface IConnectionGrain : IGrainWithGuidKey
    {
        Task Update(ConnectionSettingsDTO s);

        Task Delete();
    }
}
