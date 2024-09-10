namespace MessageSilo.Client.Helpers
{
    public static class HtmlHelpers
    {
        public static string RenderEntityDetailRow(string label, string? value, bool isSecret = false)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var finalValue = isSecret ? $"{value[..10]}*****" : value;

            return $"<tr><td><strong>{label}</strong></td><td>{finalValue}</td></tr>";
        }
    }
}
