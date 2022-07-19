using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using SBMonitor.Core.Enums;
using SBMonitor.Core.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SBMonitor.BlazorApp.Shared
{
    public partial class ConnectionSettingsModal
    {
        [Parameter]
        public HttpClient ApiClient { get; set; }

        public Guid Id { get; internal set; }
        public string Name { get; internal set; }

        public string ConnectionString { get; internal set; }

        public BusType TypeOfBus { get; internal set; }

        public string QueueName { get; internal set; }

        public string TopicName { get; internal set; }

        public string SubscriptionName { get; internal set; }

        public ObservableCollection<PinnedPath> PinnedPathes { get; internal set; }

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
            ConnectionProps conn = null;

            PinnedPathes = new(PinnedPathes.Where(p => !string.IsNullOrEmpty(p.Path) && !string.IsNullOrWhiteSpace(p.Path)));

            switch (TypeOfBus)
            {
                case BusType.Queue:
                    conn = new ConnectionProps(QueueName)
                    {
                        Id = Id,
                        ConnectionString = ConnectionString,
                        Name = Name,
                        PinnedPathes = PinnedPathes
                    };
                    break;
                case BusType.Topic:
                    conn = new ConnectionProps(TopicName, SubscriptionName)
                    {
                        Id = Id,
                        ConnectionString = ConnectionString,
                        Name = Name,
                        PinnedPathes = PinnedPathes
                    };
                    break;
            };

            var response = await ApiClient.PostAsJsonAsync("MessageMonitor/Upsert", conn);

            response.EnsureSuccessStatusCode();

            OnConnectionChanged(new ConnectionChangedEventArgs(conn));

            await ConnectionModal.Close(CloseReason.UserClosing);
        }

        public void AddPath()
        {
            PinnedPathes.Add(new());
        }

        public async Task Show(ConnectionProps? props = null)
        {
            Id = props?.Id ?? Guid.NewGuid();
            Name = props?.Name ?? "test";//string.Empty;
            ConnectionString = props?.ConnectionString ?? "Endpoint=sb://sbm-test1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=rqzi6lkkXatcCxiPNWrP3Zk5Cz8Bc8tmI9vOPtHxDMo=";//string.Empty;
            TypeOfBus = props?.TypeOfBus ?? BusType.Queue;
            QueueName = props?.QueueName ?? "kiscica";//string.Empty;
            TopicName = props?.TopicName ?? string.Empty;
            SubscriptionName = props?.SubscriptionName ?? string.Empty;
            PinnedPathes = new ObservableCollection<PinnedPath>(props == null ? Enumerable.Empty<PinnedPath>() : props.PinnedPathes);

            Disabled = props != null;

            await ConnectionModal.Show();
        }
    }
}
