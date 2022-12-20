using MessageSilo.BlazorApp.Components.DeadLetterCorrector;
using MessageSilo.Features.DeadLetterCorrector;
using MessageSilo.Shared.Models;

namespace MessageSilo.BlazorApp.Services
{
    public interface IMessageSiloAPIService
    {
        Task<List<ConnectionSettingsDTO>> GetDeadLetterCorrectors();

        Task<ConnectionSettingsDTO> GetDeadLetterCorrector(Guid id);

        Task<List<CorrectedMessage>> GetCorrectedMessages(Guid dcId, DateTimeOffset from, DateTimeOffset to);
    }
}
