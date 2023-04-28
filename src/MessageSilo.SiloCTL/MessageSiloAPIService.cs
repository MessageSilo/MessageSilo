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

        public IEnumerable<ConnectionState> GetConnections()
        {
            var result = httpClient.GetJson<IEnumerable<ConnectionState>>($"Connections");
            return result!;
        }

        public ConnectionState? GetConnection(string name)
        {
            var result = httpClient.GetJson<ConnectionState>($"Connections/{name}");
            return result;
        }

        public void UpdateConnection(ConnectionSettingsDTO dto)
        {
            httpClient.PutJson<ConnectionSettingsDTO>($"Connections/{dto.RowKey}", dto);
        }

        public IEnumerable<CorrectedMessage> GetMessages(string name, DateTimeOffset from, DateTimeOffset to)
        {
            var result = httpClient.GetJson<IEnumerable<CorrectedMessage>>($"Connections/{name}/Messages?from={from.ToString("yyyy-MM-dd HH:mm")}&to={to.ToString("yyyy-MM-dd HH:mm")}");
            return result!;
        }

        public void DeleteConnection(string name)
        {
            var request = new RestRequest($"Connections/{name}", Method.Delete);
            httpClient.Delete(request);
        }

        public IEnumerable<TargetDTO> GetTargets()
        {
            var result = httpClient.GetJson<IEnumerable<TargetDTO>>($"Targets");
            return result!;
        }

        public TargetDTO? GetTarget(string name)
        {
            var result = httpClient.GetJson<TargetDTO>($"Targets/{name}");
            return result;
        }

        public void UpdateTarget(TargetDTO dto)
        {
            httpClient.PutJson<TargetDTO>($"Targets/{dto.RowKey}", dto);
        }

        public void DeleteTarget(string name)
        {
            var request = new RestRequest($"Targets/{name}", Method.Delete);
            httpClient.Delete(request);
        }
    }
}
