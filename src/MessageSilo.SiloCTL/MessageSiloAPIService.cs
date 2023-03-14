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

        public IEnumerable<ConnectionState> GetConnections(string token)
        {
            var result = httpClient.GetJson<IEnumerable<ConnectionState>>($"User/{token}/Connections");
            return result!;
        }

        public ConnectionState? GetConnection(string token, string name)
        {
            var result = httpClient.GetJson<ConnectionState>($"User/{token}/Connections/{name}");
            return result;
        }

        public void UpdateConnection(ConnectionSettingsDTO dto)
        {
            httpClient.PutJson<ConnectionSettingsDTO>($"User/{dto.Token}/Connections/{dto.Name}", dto);
        }

        public IEnumerable<CorrectedMessage> GetMessages(string token, string name, DateTimeOffset from, DateTimeOffset to)
        {
            var result = httpClient.GetJson<IEnumerable<CorrectedMessage>>($"User/{token}/Connections/{name}/Messages?from={from.ToString("yyyy-MM-dd HH:mm")}&to={to.ToString("yyyy-MM-dd HH:mm")}");
            return result!;
        }

        public void DeleteConnection(string token, string name)
        {
            var request = new RestRequest($"User/{token}/Connections/{name}", Method.Delete);
            httpClient.Delete(request);
        }
    }
}
