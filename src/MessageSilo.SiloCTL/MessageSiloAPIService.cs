using MessageSilo.Features.MessageCorrector;
using MessageSilo.Shared.Models;
using Microsoft.Azure.Amqp.Framing;
using RestSharp;

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

        public IEnumerable<TargetDTO> GetTargets(string token)
        {
            var result = httpClient.GetJson<IEnumerable<TargetDTO>>($"User/{token}/Targets");
            return result!;
        }

        public TargetDTO? GetTarget(string token, string name)
        {
            var result = httpClient.GetJson<TargetDTO>($"User/{token}/Targets/{name}");
            return result;
        }

        public void UpdateTarget(TargetDTO dto)
        {
            httpClient.PutJson<TargetDTO>($"User/{dto.Token}/Targets/{dto.Name}", dto);
        }

        public void DeleteTarget(string token, string name)
        {
            var request = new RestRequest($"User/{token}/Targets/{name}", Method.Delete);
            httpClient.Delete(request);
        }
    }
}
