using Blazorise;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Platforms;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace MessageSilo.BlazorApp.Shared
{
    public partial class ConnectionSettingsModal
    {
        [Parameter]
        public HttpClient ApiClient { get; set; }

        public Modal? ConnectionModal;

        public ConnectionSettingsDTO ConnectionSettings { get; set; } = new ConnectionSettingsDTO();

        public IEnumerable<IPlatform> Platforms => new List<IPlatform>() { new AzurePlatform(), new AWSPlatform() };

        private IPlatform selectedValue;

        public IPlatform SelectedPlatform
        {
            get { return selectedValue; }
            internal set
            {
                selectedValue = value;
                StateHasChanged();
            }
        }

        public async Task Show()
        {
            await ConnectionModal!.Show();
        }
    }
}
