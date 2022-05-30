using Microsoft.AspNetCore.Components;
using SBMonitor.Core.Enums;
using SBMonitor.Core.Models;

namespace SBMonitor.BlazorApp.Shared
{
    public partial class ConnectionSettings
    {
        private HttpClient _apiClient;

        string Name { get; set; } = "test";

        string ConnectionString { get; set; } = "";

        BusType TypeOfBus { get; set; } = BusType.Queue;

        string QueueName { get; set; } = "kiscica";

        string TopicName { get; set; } = string.Empty;

        string SubscriptionName { get; set; } = string.Empty;

        async Task Connect()
        {
            _apiClient = _clientFactory.CreateClient("API");

            switch (TypeOfBus)
            {
                case BusType.Queue:
                    await _apiClient.PostAsJsonAsync("MessageMonitor/ConnectToQueue", new QueueConnectionProps()
                    {
                        QueueName = QueueName,
                        ConnectionString = ConnectionString,
                        Name = Name
                    });
                    break;
                case BusType.Topic:
                    await _apiClient.PostAsJsonAsync("MessageMonitor/ConnectToTopic", new TopicConnectionProps()
                    {
                        TopicName = TopicName,
                        SubscriptionName = SubscriptionName,
                        ConnectionString = ConnectionString,
                        Name = Name
                    });
                    break;
            };
        }
    }
}
