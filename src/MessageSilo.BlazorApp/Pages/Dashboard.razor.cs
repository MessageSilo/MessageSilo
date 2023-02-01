using MessageSilo.BlazorApp.Services;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace MessageSilo.BlazorApp.Pages
{
    public partial class Dashboard
    {
        [Inject]
        public IMessageSiloAPIService MessageSiloAPI { get; set; }

        public IEnumerable<ConnectionSettingsDTO> Connections { get; set; } = new List<ConnectionSettingsDTO>();

        protected override async Task OnInitializedAsync()
        {
            Connections = (await MessageSiloAPI.GetConnections()).OrderBy(p => p.Name);
        }

        public async Task Delete(Guid id)
        {
            await MessageSiloAPI.DeleteConnection(id);
            Connections = Connections.Where(p => p.Id != id).ToList();
        }
    }
}
