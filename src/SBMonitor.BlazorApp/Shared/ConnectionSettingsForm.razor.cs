using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using SBMonitor.Core.Enums;
using SBMonitor.Core.Models;
using System.Net.Http.Json;

namespace SBMonitor.BlazorApp.Shared
{
    public partial class ConnectionSettingsForm
    {
        [Parameter]
        public HttpClient ApiClient { get; set; }

        public string Name { get; set; } = "test";

        public string ConnectionString { get; set; } = "Endpoint=sb://sbm-test1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=rqzi6lkkXatcCxiPNWrP3Zk5Cz8Bc8tmI9vOPtHxDMo=";

        public BusType TypeOfBus { get; set; } = BusType.Queue;

        public string QueueName { get; set; } = "kiscica";

        public string TopicName { get; set; } = string.Empty;

        public string SubscriptionName { get; set; } = string.Empty;

        public async Task<ConnectionProps?> ConnectAsync()
        {
            switch (TypeOfBus)
            {
                case BusType.Queue:
                    var queueResponse = await ApiClient.PostAsJsonAsync("MessageMonitor/ConnectToQueue", new QueueConnectionProps()
                    {
                        Id = Guid.NewGuid(),
                        QueueName = QueueName,
                        ConnectionString = ConnectionString,
                        Name = Name
                    });
                    return await queueResponse.Content.ReadFromJsonAsync<QueueConnectionProps>();
                case BusType.Topic:
                    var topicResponse = await ApiClient.PostAsJsonAsync("MessageMonitor/ConnectToTopic", new TopicConnectionProps()
                    {
                        Id = Guid.NewGuid(),
                        TopicName = TopicName,
                        SubscriptionName = SubscriptionName,
                        ConnectionString = ConnectionString,
                        Name = Name
                    });
                    return await topicResponse.Content.ReadFromJsonAsync<TopicConnectionProps>();
            };

            return null;
        }
    }
}
