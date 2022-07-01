using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SBMonitor.Core.Extensions
{
    public static class JsonElementExtensions
    {
        public static JsonElement GetJsonElement(this JsonElement jsonElement, string path)
        {
            if (jsonElement.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
                return default;

            string[] segments = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in segments)
            {
                if (int.TryParse(segment, out var index) && jsonElement.ValueKind == JsonValueKind.Array)
                {
                    jsonElement = jsonElement.EnumerateArray().ElementAtOrDefault(index);
                    if (jsonElement.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
                        return default;

                    continue;
                }

                jsonElement = jsonElement.TryGetProperty(segment, out var value) ? value : default;

                if (jsonElement.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
                    return default;
            }

            return jsonElement;
        }

        public static string? GetJsonElementValue(this JsonElement jsonElement) => jsonElement.ValueKind != JsonValueKind.Null &&
                                                                                   jsonElement.ValueKind != JsonValueKind.Undefined
            ? jsonElement.ToString()
            : default;
    }
}
