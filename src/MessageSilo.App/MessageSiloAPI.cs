using MessageSilo.Shared.Models;
using Microsoft.Azure.Amqp.Transaction;
using RestSharp;
using System.Net;
using System.Net.Http;

namespace MessageSilo.App
{
    public class MessageSiloAPI
    {
        private readonly RestClient httpClient;

        public MessageSiloAPI(RestClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ApiContract<IEnumerable<Entity>>> GetEntities()
        {
            var result = await httpClient.GetJsonAsync<ApiContract<IEnumerable<Entity>>>("Entities");
            return result;
        }

        public async Task<ApiContract<IEnumerable<R>>> Get<R>(string controller) where R : class
        {
            var result = await httpClient.GetJsonAsync<ApiContract<IEnumerable<R>>>(controller);
            return result;
        }

        public async Task<ApiContract<LastMessage>> GetLastMessage(string controller, string name)
        {
            var result = await httpClient.GetJsonAsync<ApiContract<LastMessage>>($"{controller}/{name}/last-message");
            return result;
        }

        public async Task<ApiContract<R>> Get<R>(string controller, string name) where R : class
        {
            var result = await httpClient.GetJsonAsync<ApiContract<R>>($"{controller}/{name}");
            return result;
        }

        public async Task<ApiContract<R>> Update<DTO, R>(string controller, DTO dto) where DTO : Entity where R : class
        {
            var result = await httpClient.PutJsonAsync<DTO, ApiContract<R>>($"{controller}/{dto.RowKey}", dto);
            return result;
        }

        public async Task<ApiContract<R>> Delete<R>(string controller, string name) where R : class
        {
            var request = new RestRequest($"{controller}/{name}", Method.Delete);
            return await httpClient.DeleteAsync<ApiContract<R>>(request);
        }
    }
}
