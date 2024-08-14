using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;
using RestSharp;

namespace MessageSilo.SiloCTL
{
    public class MessageSiloAPI
    {
        private readonly RestClient httpClient;

        public MessageSiloAPI(RestClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public IEnumerable<Entity> List()
        {
            var result = httpClient.GetJson<IEnumerable<Entity>>("Entities");
            return result;
        }

        public void Clear()
        {
            var request = new RestRequest($"Entities", Method.Delete);
            httpClient.Delete(request);
        }

        public IEnumerable<EntityValidationErrors>? Apply(ApplyDTO dto)
        {
            var result = httpClient.PostJson<ApplyDTO, IEnumerable<EntityValidationErrors>?>("Entities", dto);
            return result;
        }
    }
}
