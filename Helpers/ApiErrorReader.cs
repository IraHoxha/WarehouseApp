using System.Text.Json;

namespace warehouseapp.Helpers
{
    public static class ApiErrorReader
    {
        public static async Task<string> ReadMessageAsync(HttpResponseMessage res)
        {
            if (res.Content == null)
                return "Operation failed.";

            var content = await res.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
                return $"Request failed ({(int)res.StatusCode}).";

            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                    doc.RootElement.TryGetProperty("message", out var msg))
                {
                    var m = msg.GetString();
                    if (!string.IsNullOrWhiteSpace(m))
                        return m!;
                }
            }
            catch
            {
                // ignore (not JSON object)
            }

            try
            {
                var s = JsonSerializer.Deserialize<string>(content);
                if (!string.IsNullOrWhiteSpace(s))
                    return s!;
            }
            catch
            {
            }

            return content.Trim().Trim('"');
        }
    }
}
