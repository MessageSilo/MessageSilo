using MessageSilo.Features.Connection;
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

        public async Task<List<ConnectionSettingsDTO>> GetConnections()
        {
            var result = await httpClient.GetFromJsonAsync<List<ConnectionSettingsDTO>>("api/v1/Connection");

            return result!;
        }

        public async Task<ConnectionSettingsDTO> GetConnection(Guid id)
        {
            var result = await httpClient.GetFromJsonAsync<ConnectionSettingsDTO>($"api/v1/Connection/{id}");

            return result!;
        }

        public async Task UpsertConnection(ConnectionSettingsDTO dto)
        {
            var result = await httpClient.PutAsJsonAsync<ConnectionSettingsDTO>($"api/v1/Connection", dto);
        }

        public async Task<List<CorrectedMessage>> GetCorrectedMessages(Guid dcId, DateTimeOffset from, DateTimeOffset to)
        {
            var result = await httpClient.GetFromJsonAsync<IEnumerable<CorrectedMessage>>($"api/v1/Connection/{dcId}/Messages?from={from.ToString("yyyy-MM-dd HH:mm")}&to={to.ToString("yyyy-MM-dd HH:mm")}");

            return result!.ToList();
        }

        public async Task DeleteConnection(Guid id)
        {
            var result = await httpClient.DeleteAsync($"api/v1/Connection/{id}");
        }
    }
}
