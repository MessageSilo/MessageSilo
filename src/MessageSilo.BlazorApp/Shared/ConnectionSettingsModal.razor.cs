using Blazorise;
using MessageSilo.Shared.Platforms;
using Microsoft.AspNetCore.Components;

namespace MessageSilo.BlazorApp.Shared
{
    public partial class ConnectionSettingsModal
    {
        [Parameter]
        public HttpClient ApiClient { get; set; }

        private Modal? ConnectionModal;

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
    }
}
