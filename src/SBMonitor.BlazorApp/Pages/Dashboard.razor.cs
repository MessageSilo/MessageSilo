using Blazorise;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using SBMonitor.BlazorApp.Shared;
using SBMonitor.Core.Models;
using System.Net.Http.Json;

namespace SBMonitor.BlazorApp.Pages
{
    public partial class Dashboard
    {
        private ConnectionSettingsModal ConnectionSettingsModal;

        private List<ConnectionProps> Items = new List<ConnectionProps>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ConnectionSettingsModal.ConnectionChanged += ConnectionSettingsModal_ConnectionChanged;

                Items = new List<ConnectionProps>();
            }

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
