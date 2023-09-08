using MessageSilo.Shared.Models;
using RestSharp;

namespace MessageSilo.Features.Target
{
    public class APITarget : ITarget
    {
        private readonly string url;

        private readonly Method method = Method.Post;

        private IRestClient client = new RestClient();

        public APITarget(string url)
        {
            this.url = url;
        }

        public async Task Send(Message message)
        {
            var request = new RestRequest(url, method);
            request.AddBody(message.Body, contentType: ContentType.Json);

            var response = await client.ExecutePostAsync(request);

            if (!response.IsSuccessful)
                throw response.ErrorException;
        }
    }
}
