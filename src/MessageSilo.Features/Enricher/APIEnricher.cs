using Newtonsoft.Json.Linq;
using RestSharp;
using System.Text.RegularExpressions;

namespace MessageSilo.Features.Enricher
{
    public class APIEnricher : IEnricher
    {
        private readonly string url;

        private readonly Method method;

        private Regex rg = new Regex("{[a-zA-Z0-9_.-]*}");

        private IRestClient client = new RestClient();

        public APIEnricher(string url, Method method)
        {
            this.url = url;
            this.method = method;
        }

        public async Task<string> TransformMessage(string message)
        {
            var request = new RestRequest(url, method);

            if (method == Method.Post || method == Method.Put)
                request.AddBody(message, contentType: ContentType.Json);
            else
            {
                var matches = rg.Matches(url);
                var replacedURL = url;
                var messageObj = JObject.Parse(message);

                foreach (Match match in matches)
                {
                    var propPath = match.Value.TrimStart('{').TrimEnd('}');
                    var prop = messageObj.SelectToken(propPath);

                    if (prop is not null)
                        replacedURL = replacedURL.Replace(match.Value, prop.Value<string>());
                }

                request = new RestRequest(replacedURL, method);
            }

            var response = await client.ExecuteAsync(request);

            return response.Content;
        }
    }
}
