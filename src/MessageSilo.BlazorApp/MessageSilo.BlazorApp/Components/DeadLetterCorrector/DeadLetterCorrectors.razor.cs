using MessageSilo.BlazorApp.Services;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace MessageSilo.BlazorApp.Components.DeadLetterCorrector
{
    public partial class DeadLetterCorrectors
    {
        [Inject]
        public IMessageSiloAPIService MessageSiloAPI { get; set; }

        public IEnumerable<ConnectionSettingsDTO> Connections { get; set; } = new List<ConnectionSettingsDTO>();

        protected override async Task OnInitializedAsync()
        {
            Connections = (await MessageSiloAPI.GetDeadLetterCorrectors()).OrderBy(p => p.Name);
        }
    }
}
