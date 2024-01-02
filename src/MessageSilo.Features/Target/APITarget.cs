using MessageSilo.Shared.Models;
using Polly;
using Polly.Retry;
using RestSharp;

namespace MessageSilo.Features.Target
{
    public class APITarget : ITarget
    {
        private readonly string url;

        private readonly Method method = Method.Post;

        private IRestClient client = new RestClient();

        private readonly ResiliencePipeline<RestResponse> pipeline;

        public APITarget(string url, RetrySettings retrySettings)
        {
            this.url = url;
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

        public async Task Send(Message message)
        {
            var request = new RestRequest(url, method);
            request.AddBody(message.Body, contentType: ContentType.Json);

            var response = await pipeline.ExecuteAsync(async token =>
            {
                return await client.ExecutePostAsync(request, cancellationToken: token);
            });

            if (!response.IsSuccessful)
                throw response.ErrorException;
        }
    }
}
