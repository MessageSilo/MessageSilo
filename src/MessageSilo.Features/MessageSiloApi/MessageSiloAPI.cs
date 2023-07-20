using MessageSilo.Shared.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Features.MessageSiloApi
{
    public class MessageSiloAPI : IMessageSiloAPI
    {
        private readonly RestClient httpClient;

        public MessageSiloAPI(RestClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public ApiContract<IEnumerable<Entity>> GetEntities()
        {
            var result = httpClient.GetJson<ApiContract<IEnumerable<Entity>>>("Entities");
            return result!;
        }

        public ApiContract<IEnumerable<R>> Get<R>(string controller) where R : class
        {
            var result = httpClient.GetJson<ApiContract<IEnumerable<R>>>(controller);
            return result!;
        }

        public ApiContract<LastMessage> GetLastMessage(string controller, string name)
        {
            var result = httpClient.GetJson<ApiContract<LastMessage>>($"{controller}/{name}/last-message");
            return result!;
        }

        public ApiContract<R> Get<R>(string controller, string name) where R : class
        {
            var result = httpClient.GetJson<ApiContract<R>>($"{controller}/{name}");
            return result!;
        }

        public ApiContract<R> Update<DTO, R>(string controller, DTO dto) where DTO : Entity where R : class
        {
            var result = httpClient.PutJson<DTO, ApiContract<R>>($"{controller}/{dto.RowKey}", dto);
            return result!;
        }

        public ApiContract<R> Delete<R>(string controller, string name) where R : class
        {
            var request = new RestRequest($"{controller}/{name}", Method.Delete);
            return httpClient.Delete<ApiContract<R>>(request)!;
        }
    }
}
