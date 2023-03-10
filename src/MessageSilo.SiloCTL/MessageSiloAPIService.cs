using Azure.Core;
using Azure;
using MessageSilo.Features.MessageCorrector;
using MessageSilo.Shared;
using MessageSilo.Shared.Models;
using RestSharp;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;

namespace MessageSilo.SiloCTL
{
    public class MessageSiloAPIService
    {
        private readonly RestClient httpClient;

        public MessageSiloAPIService(RestClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public IEnumerable<string> GetConnections(string token)
        {
            var result = httpClient.GetJson<List<string>>($"User/{token}/Connections");
            return result!;
        }

        public ConnectionState GetConnection(string id)
        {
            var result = httpClient.GetJson<ConnectionState>($"Connections/{id}");
            return result!;
        }

        public void UpdateConnection(ConnectionSettingsDTO dto)
        {
            httpClient.PutJson<ConnectionSettingsDTO>($"Connections/{dto.Id}", dto);
        }

        public IEnumerable<CorrectedMessage> GetMessages(string connId, DateTimeOffset from, DateTimeOffset to)
        {
            var result = httpClient.GetJson<IEnumerable<CorrectedMessage>>($"Connections/{connId}/Messages?from={from.ToString("yyyy-MM-dd HH:mm")}&to={to.ToString("yyyy-MM-dd HH:mm")}");
            return result!;
        }

        public void DeleteConnection(string id)
        {
            var request = new RestRequest($"Connections/{id}", Method.Delete);
            httpClient.Delete(request);
        }
    }
}
