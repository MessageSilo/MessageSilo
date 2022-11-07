using Blazorise;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using SBMonitor.BlazorApp.Shared;
using SBMonitor.Core.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace SBMonitor.BlazorApp.Pages
{
    public partial class Dashboard
    {
        private ConnectionSettingsModal ConnectionSettingsModal;

        private ObservableCollection<ConnectionProps> Items = new ObservableCollection<ConnectionProps>();

        private int rows = 2;

        public void RefreshState()
        {
            InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            Items = await ApiClient.GetFromJsonAsync<ObservableCollection<ConnectionProps>>("MessageMonitor/Connections");
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ConnectionSettingsModal.ConnectionChanged += ConnectionSettingsModal_ConnectionChanged;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private void ConnectionSettingsModal_ConnectionChanged(object? sender, EventArgs e)
        {
            var ea = e as ConnectionChangedEventArgs;

            if (ea == null)
                return;

            var existingConn = Items.FirstOrDefault(p => p.Id == ea.ConnectionProps.Id);

            if (existingConn == null)
                Items.Add(ea.ConnectionProps);
            else
                existingConn.Update(ea.ConnectionProps);

            RefreshState();
        }

        private async Task DeleteItem(Guid id)
        {
            var response = await ApiClient.DeleteAsync($"MessageMonitor/Delete?id={id}");

            response.EnsureSuccessStatusCode();

            Items.Remove(Items.First(p => p.Id == id));
        }

        private async Task SetRows(int rows)
        {
            this.rows = rows;
        }
    }
}
