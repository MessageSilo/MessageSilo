using MessageSilo.Domain.Interfaces;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using RestSharp;
using System.Text.RegularExpressions;

namespace MessageSilo.Domain.Entities
{
    public class APIEnricher : IEnricher
    {
        private readonly string url;

        private readonly Method method;

        private readonly Regex rg = new Regex("{[a-zA-Z0-9_.-]*}");

        private readonly IRestClient client = new RestClient();

        private readonly ResiliencePipeline<RestResponse> pipeline;

        public APIEnricher(string url, Method method, RetrySettings retrySettings)
        {
            this.url = url;
            this.method = method;
            pipeline = new ResiliencePipelineBuilder<RestResponse>()
                .AddRetry(new RetryStrategyOptions<RestResponse>
                {
                    ShouldHandle = new PredicateBuilder<RestResponse>().HandleResult(r => !r.IsSuccessful),
                    MaxRetryAttempts = retrySettings.MaxRetryAttempts,
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                })
                .Build();
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

            var response = await pipeline.ExecuteAsync(async token =>
            {
                return await client.ExecuteAsync(request, cancellationToken: token);
            });

            if (!response.IsSuccessful)
                throw response.ErrorException;

            return response.Content;
        }
    }
}
