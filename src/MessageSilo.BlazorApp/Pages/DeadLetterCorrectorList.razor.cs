using Blazorise;
using MessageSilo.Shared.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace MessageSilo.BlazorApp.Pages
{
    public partial class DeadLetterCorrectorList
    {
        public List<ConnectionSettingsDTO> Connections = new List<ConnectionSettingsDTO>();

        protected override async Task OnInitializedAsync()
        {
            Connections = await ApiClient.GetFromJsonAsync<List<ConnectionSettingsDTO>>("api/v1/DeadLetterCorrector");
            await base.OnInitializedAsync();
        }
    }
}
