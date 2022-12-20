using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Components;
using System.Text.Json;

namespace MessageSilo.BlazorApp.Components
{
    public partial class MessageDisplayer
    {
        [Parameter]
        public string OriginalMessage { get; set; }

        public string FormattedMessage { get; set; }

        protected override Task OnParametersSetAsync()
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(OriginalMessage);

            FormattedMessage = JsonSerializer.Serialize(jsonElement, options);

            return base.OnParametersSetAsync();
        }
    }
}
