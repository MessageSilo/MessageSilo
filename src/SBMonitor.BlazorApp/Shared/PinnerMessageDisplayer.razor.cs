using Microsoft.AspNetCore.Components;
using SBMonitor.Core.Extensions;
using SBMonitor.Core.Models;
using System.Text.Json;

namespace SBMonitor.BlazorApp.Shared
{
    public partial class PinnerMessageDisplayer
    {
        [Parameter]
        public string InputMessage { get; set; }

        [Parameter]
        public IList<PinnedPath> PinnedPathes { get; set; }

        private Dictionary<string, string?> PinnedItems { get; set; } = new Dictionary<string, string?>();

        protected override async Task OnParametersSetAsync()
        {
            if (PinnedPathes?.Count > 0 && !string.IsNullOrWhiteSpace(InputMessage))
            {
                var jObject = JsonSerializer.Deserialize<JsonElement>(InputMessage);

                foreach (var pp in PinnedPathes)
                {
                    var value = jObject.GetJsonElement(pp.Path).GetJsonElementValue();
                    if (PinnedItems.ContainsKey(pp.Path))
                        PinnedItems[pp.Path] = value;
                    else
                        PinnedItems.Add(pp.Path, value);
                }

                await InvokeAsync(StateHasChanged);
            }

            await base.OnParametersSetAsync();
        }
    }
}
