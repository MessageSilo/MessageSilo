using Blazorise;
using Microsoft.AspNetCore.SignalR.Client;
using SBMonitor.BlazorApp.Shared;
using SBMonitor.Core.Models;

namespace SBMonitor.BlazorApp.Pages
{
    public partial class Dashboard
    {
        private Modal NewConnectionModal;

        private ConnectionSettingsForm ConnectionSettings;

        private HttpClient ApiClient;

        private List<ConnectionProps> Items = new List<ConnectionProps>();

        protected override async Task OnInitializedAsync()
        {
            ApiClient = _clientFactory.CreateClient("API");
            await base.OnInitializedAsync();
        }

        private async Task AddNewConnection()
        {
            var newConnection = await ConnectionSettings.ConnectAsync();

            if (newConnection != null)
                Items.Add(newConnection);

            await NewConnectionModal.Close(CloseReason.UserClosing);
        }
    }
}
