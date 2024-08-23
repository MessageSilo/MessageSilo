using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;
using System.Net.Http.Json;

namespace MessageSilo.SiloCTL
{
    public class MessageSiloAPI
    {
        private readonly HttpClient httpClient;

        public MessageSiloAPI(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public IEnumerable<Entity> List()
        {
            var response = httpClient.GetFromJsonAsync<IEnumerable<Entity>>("Entities").GetAwaiter().GetResult();
            return response;
        }

        public void Clear()
        {
            var response = httpClient.DeleteAsync("Entities").GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
        }

        public IEnumerable<EntityValidationErrors>? Apply(ApplyDTO dto)
        {
            var response = httpClient.PostAsJsonAsync("Entities", dto).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var result = response.Content.ReadFromJsonAsync<IEnumerable<EntityValidationErrors>?>().GetAwaiter().GetResult();

            return result;
        }
    }
}
