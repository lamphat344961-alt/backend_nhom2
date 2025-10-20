namespace backend_nhom2.Services.Geo
{
    /// <summary>
    /// Geocoding qua OpenStreetMap Nominatim (free). Yêu cầu header User-Agent.
    /// </summary>
    public class GeocodingService
    {
        private readonly HttpClient _http;

        public GeocodingService(HttpClient http)
        {
            _http = http;
            // Nominatim yêu cầu User-Agent
            if (!_http.DefaultRequestHeaders.Contains("User-Agent"))
                _http.DefaultRequestHeaders.Add("User-Agent", "backend_nhom2/1.0 (+https://example.local)");
        }

        public async Task<(double lat, double lng)?> GeocodeAsync(string address, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(address)) return null;

            var url = $"https://nominatim.openstreetmap.org/search?format=json&limit=1&q={Uri.EscapeDataString(address)}";
            using var req = new HttpRequestMessage(HttpMethod.Get, url);

            using var resp = await _http.SendAsync(req, ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadFromJsonAsync<List<NominatimSearchItem>>(cancellationToken: ct);
            var first = json?.FirstOrDefault();
            if (first == null) return null;

            if (double.TryParse(first.lat, System.Globalization.CultureInfo.InvariantCulture, out var la) &&
                double.TryParse(first.lon, System.Globalization.CultureInfo.InvariantCulture, out var lo))
            {
                return (la, lo);
            }
            return null;
        }

        private class NominatimSearchItem
        {
            public string? display_name { get; set; }
            public string lat { get; set; } = "";
            public string lon { get; set; } = "";
        }
    }
}
