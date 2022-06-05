using Microsoft.AspNetCore.SignalR.Client;
using SBMonitor.Core.Enums;
using SBMonitor.Core.Models;

namespace SBMonitor.BlazorApp.Shared
{
    public partial class ConnectionSettings
    {
        private HttpClient _apiClient;

        private HubConnection _hubConnection;

        string Name { get; set; } = "test";

        string ConnectionString { get; set; } = "";

        BusType TypeOfBus { get; set; } = BusType.Queue;

        string QueueName { get; set; } = "kiscica";

        string TopicName { get; set; } = string.Empty;

        string SubscriptionName { get; set; } = string.Empty;

        private void _hubConnection_Received(string obj)
        {
            Name = obj;

            StateHasChanged();
        }

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

            _hubConnection = new HubConnectionBuilder().WithUrl($"{_apiClient.BaseAddress}MessageMonitorHub").Build();

            _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                Name = encodedMsg;
                StateHasChanged();
            });

            await _hubConnection.StartAsync();
        }
    }
}
