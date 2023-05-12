using MessageSilo.Shared.Models;
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

        #region Connections

        public IEnumerable<ConnectionState> GetConnections()
        {
            var result = httpClient.GetJson<ApiContract<IEnumerable<ConnectionState>>>($"Connections");

            return result.Data;
        }

        public ConnectionState? GetConnection(string name)
        {
            var result = httpClient.GetJson<ApiContract<ConnectionState>>($"Connections/{name}");
            return result.Data;
        }

        public void UpdateConnection(ConnectionSettingsDTO dto)
        {
            httpClient.PutJson<ConnectionSettingsDTO>($"Connections/{dto.RowKey}", dto);
        }

        public void DeleteConnection(string name)
        {
            var request = new RestRequest($"Connections/{name}", Method.Delete);
            httpClient.Delete(request);
        }

        #endregion

        #region Tragets

        public IEnumerable<TargetDTO> GetTargets()
        {
            var result = httpClient.GetJson<ApiContract<IEnumerable<TargetDTO>>>($"Targets");
            return result.Data;
        }

        public TargetDTO? GetTarget(string name)
        {
            var result = httpClient.GetJson<ApiContract<TargetDTO>>($"Targets/{name}");
            return result.Data;
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

        #endregion

        #region Enrichers

        public IEnumerable<EnricherDTO> GetEnrichers()
        {
            var result = httpClient.GetJson<ApiContract<IEnumerable<EnricherDTO>>>($"Enrichers");
            return result.Data;
        }

        public EnricherDTO? GetEnricher(string name)
        {
            var result = httpClient.GetJson<ApiContract<EnricherDTO>>($"Enrichers/{name}");
            return result.Data;
        }

        public void UpdateEnricher(EnricherDTO dto)
        {
            httpClient.PutJson<EnricherDTO>($"Enrichers/{dto.RowKey}", dto);
        }

        public void DeleteEnricher(string name)
        {
            var request = new RestRequest($"Enrichers/{name}", Method.Delete);
            httpClient.Delete(request);
        }

        #endregion
    }
}
