using Microsoft.AspNetCore.Components;
using SBMonitor.Core.Extensions;
using System.Text.Json;

namespace SBMonitor.BlazorApp.Shared
{
    public partial class DefaultJsonMessageDisplayer
    {
        [Parameter]
        public string InputMessage { get; set; }

        string FormattedMessage { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrWhiteSpace(InputMessage))
            {
                var json = JsonSerializer.Serialize(InputMessage, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                json = json.Replace(@"\r\n", "<br>");
                json = json.Replace(@"\u0022", "\"");
                json = json.Replace(@"  ", "&emsp;");

                FormattedMessage = json;

                //var yourObject = JsonSerializer.Deserialize<JsonElement>(InputMessage);
                //var asd = yourObject.GetJsonElement("name");
            }

            await base.OnParametersSetAsync();
        }
    }
}
