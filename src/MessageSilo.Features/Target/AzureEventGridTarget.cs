using Azure;
using Azure.Messaging.EventGrid;
using MessageSilo.Shared.Models;
using RestSharp;

namespace MessageSilo.Features.Target
{
    public class AzureEventGridTarget : ITarget
    {
        private readonly string endpoint;

        private readonly string topicName;

        private readonly string accessKey;

        private EventGridPublisherClient client;

        public AzureEventGridTarget(string endpoint, string topicName, string accessKey)
        {
            this.endpoint = endpoint;
            this.topicName = topicName;
            this.accessKey = accessKey;

            client = new EventGridPublisherClient(
                 new Uri(endpoint),
                 new AzureKeyCredential(accessKey));
        }

        public async Task Send(Message message)
        {
            //EventGridEvent egEvent = new EventGridEvent();
        }
    }
}
