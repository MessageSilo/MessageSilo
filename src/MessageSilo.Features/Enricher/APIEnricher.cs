using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.Features.Enricher
{
    public class APIEnricher : IEnricher
    {
        private readonly string url;

        private IRestClient client = new RestClient();

        public APIEnricher(string url)
        {
            this.url = url;
        }

        public async Task<string> TransformMessage(string message)
        {
            var request = new RestRequest(url, Method.Post);
            request.AddBody(message, contentType: ContentType.Json);

            var response = await client.ExecutePostAsync(request);

            return response.Content;
        }
    }
}
