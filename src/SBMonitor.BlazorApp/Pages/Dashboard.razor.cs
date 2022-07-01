using Blazorise;
using Microsoft.AspNetCore.SignalR.Client;
using SBMonitor.BlazorApp.Shared;
using SBMonitor.Core.Models;

namespace SBMonitor.BlazorApp.Pages
{
    public partial class Dashboard
    {
        private ConnectionSettingsModal ConnectionSettingsModal;

        private HttpClient ApiClient;

        private List<ConnectionProps> Items = new List<ConnectionProps>();

        protected override async Task OnInitializedAsync()
        {
            var asd = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            ApiClient = _clientFactory.CreateClient("API");
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                ConnectionSettingsModal.ConnectionChanged += ConnectionSettingsModal_ConnectionChanged;

            await base.OnAfterRenderAsync(firstRender);
        }

        private void ConnectionSettingsModal_ConnectionChanged(object? sender, EventArgs e)
        {
            var ea = e as ConnectionChangedEventArgs;

            if (ea != null)
            {
                Items.Add(ea.ConnectionProps);
                InvokeAsync(StateHasChanged);
            }
        }
    }
}
