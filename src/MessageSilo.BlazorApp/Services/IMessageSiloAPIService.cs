using MessageSilo.Features.MessageCorrector;
using MessageSilo.Shared.Models;

namespace MessageSilo.BlazorApp.Services
{
    public interface IMessageSiloAPIService
    {
        Task<List<ConnectionSettingsDTO>> GetConnections();

        Task<ConnectionSettingsDTO> GetConnection(Guid id);

        Task<List<CorrectedMessage>> GetCorrectedMessages(Guid dcId, DateTimeOffset from, DateTimeOffset to);

        Task UpsertConnection(ConnectionSettingsDTO dto);

        Task DeleteConnection(string id);
    }
}
