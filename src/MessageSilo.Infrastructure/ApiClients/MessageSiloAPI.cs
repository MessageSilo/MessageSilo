using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;
using MessageSilo.Infrastructure.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace MessageSilo.Infrastructure.ApiClients
{
    public class MessageSiloAPI : IMessageSiloAPI
    {
        private readonly HttpClient httpClient;

        public MessageSiloAPI(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri(this.httpClient.BaseAddress + "api/v1/");
        }

        public async Task<IEnumerable<Entity>> List()
        {
            var response = await httpClient.GetFromJsonAsync<IEnumerable<Entity>>("Entities");
            return response;
        }

        public async Task Clear()
        {
            var response = await httpClient.DeleteAsync("Entities");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<EntityValidationErrors>?> Apply(ApplyDTO dto)
        {
            var response = await httpClient.PostAsJsonAsync("Entities", dto);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == HttpStatusCode.NoContent)
                return null;

            var result = response.Content.ReadFromJsonAsync<IEnumerable<EntityValidationErrors>?>().GetAwaiter().GetResult();

            return result;
        }

        public async Task Send(string connectionId, MessageDTO dto)
        {
            var response = await httpClient.PostAsJsonAsync($"Connections/{connectionId}", dto);
            response.EnsureSuccessStatusCode();
        }
    }
}
