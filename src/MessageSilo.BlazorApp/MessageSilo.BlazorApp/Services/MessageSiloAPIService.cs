using MessageSilo.BlazorApp.Components.DeadLetterCorrector;
using MessageSilo.Features.DeadLetterCorrector;
using MessageSilo.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace MessageSilo.BlazorApp.Services
{
    public class MessageSiloAPIService : IMessageSiloAPIService
    {
        private readonly HttpClient httpClient;

        public MessageSiloAPIService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<ConnectionSettingsDTO>> GetDeadLetterCorrectors()
        {
            var result = await httpClient.GetFromJsonAsync<List<ConnectionSettingsDTO>>("api/v1/DeadLetterCorrector");

            return result!;
        }

        public async Task<ConnectionSettingsDTO> GetDeadLetterCorrector(Guid id)
        {
            var result = await httpClient.GetFromJsonAsync<ConnectionSettingsDTO>($"api/v1/DeadLetterCorrector/{id}");

            return result!;
        }

        public async Task<List<CorrectedMessage>> GetCorrectedMessages(Guid dcId)
        {
            var result = await httpClient.GetFromJsonAsync<IEnumerable<CorrectedMessage>>($"api/v1/DeadLetterCorrector/{dcId}/Messages");

            return result!.ToList();
        }
    }
}
