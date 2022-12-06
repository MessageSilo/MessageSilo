using Blazorise;
using MessageSilo.BlazorApp.Shared;
using MessageSilo.Shared.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace MessageSilo.BlazorApp.Pages
{
    public partial class DeadLetterCorrectorList
    {
        public List<ConnectionSettingsDTO> Connections = new List<ConnectionSettingsDTO>();

        public ConnectionSettingsModal? ConnectionSettingsModal { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Connections = await ApiClient.GetFromJsonAsync<List<ConnectionSettingsDTO>>("api/v1/DeadLetterCorrector");
            await base.OnInitializedAsync();
        }
    }
}
