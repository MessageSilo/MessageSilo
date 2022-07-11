using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using SBMonitor.Core.Enums;
using SBMonitor.Core.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SBMonitor.BlazorApp.Shared
{
    public partial class ConnectionSettingsModal
    {
        [Parameter]
        public HttpClient ApiClient { get; set; }

        public string Name { get; internal set; }

        public string ConnectionString { get; internal set; }

        public BusType TypeOfBus { get; internal set; }

        public string QueueName { get; internal set; }

        public string TopicName { get; internal set; }

        public string SubscriptionName { get; internal set; }

        public bool Disabled { get; internal set; }

        private Modal ConnectionModal;

        public event EventHandler ConnectionChanged;

        protected virtual void OnConnectionChanged(ConnectionChangedEventArgs e)
        {
            var handler = ConnectionChanged;
            handler?.Invoke(this, e);
        }

        public async Task Save()
        {
            ConnectionProps? props = null;

            switch (TypeOfBus)
            {
                case BusType.Queue:
                    props = new ConnectionProps(QueueName)
                    {
                        Id = Guid.NewGuid(),
                        ConnectionString = ConnectionString,
                        Name = Name
                    };
                    break;
                case BusType.Topic:
                    props = new ConnectionProps(TopicName, SubscriptionName)
                    {
                        Id = Guid.NewGuid(),
                        ConnectionString = ConnectionString,
                        Name = Name
                    };
                    break;
            };

            var response = await ApiClient.PostAsJsonAsync("MessageMonitor/Upsert", props);

            var cp = await response.Content.ReadFromJsonAsync<ConnectionProps>();

            if (cp != null)
                OnConnectionChanged(new ConnectionChangedEventArgs(cp));


            await ConnectionModal.Close(CloseReason.UserClosing);
        }

        public async Task Show(ConnectionProps? props = null)
        {
            Name = props?.Name ?? "test";//string.Empty;
            ConnectionString = props?.ConnectionString ?? "Endpoint=sb://sbm-test1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=rqzi6lkkXatcCxiPNWrP3Zk5Cz8Bc8tmI9vOPtHxDMo=";//string.Empty;
            TypeOfBus = props?.TypeOfBus ?? BusType.Queue;
            QueueName = props?.QueueName ?? "kiscica";//string.Empty;
            TopicName = props?.TopicName ?? string.Empty;
            SubscriptionName = props?.SubscriptionName ?? string.Empty;

            Disabled = props != null;

            await ConnectionModal.Show();
        }
    }
}
