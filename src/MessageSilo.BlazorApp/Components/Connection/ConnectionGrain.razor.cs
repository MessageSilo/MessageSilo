using Blazorise;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace MessageSilo.BlazorApp.Components.Connection
{
    public partial class ConnectionGrain
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        [Parameter]
        public ConnectionSettingsDTO ConnectionSettings { get; set; }

        public Background StatusColor { get; set; } = Background.Light;
    }
}
