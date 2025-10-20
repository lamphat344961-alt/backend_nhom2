namespace backend_nhom2.Services.Route
{
    public class OsmClients
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;

        public OsmClients(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _baseUrl = cfg["Osrm:BaseUrl"]?.TrimEnd('/') ?? "https://router.project-osrm.org";
        }

        public async Task<int[,]> BuildTimeMatrixAsync(IEnumerable<(double lat, double lng)> points)
        {
            var pts = points.ToList();
            if (pts.Count == 0) throw new ArgumentException("points empty");

            // OSRM table API: /table/v1/driving/{lng,lat;...}?annotations=duration
            string coords = string.Join(";", pts.Select(p => $"{p.lng.ToString(System.Globalization.CultureInfo.InvariantCulture)},{p.lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}"));
            var url = $"{_baseUrl}/table/v1/driving/{coords}?annotations=duration";

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            using var resp = await _http.SendAsync(req);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadFromJsonAsync<TableResponse>();
            if (json?.durations is null) throw new InvalidOperationException("OSRM table: durations null");

            int n = json.durations.Length;
            int[,] matrix = new int[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    matrix[i, j] = (int)Math.Round((json.durations[i][j] ?? 0.0));

            return matrix;
        }

        public async Task<string> GetPolylineAsync(double fromLat, double fromLng, double toLat, double toLng)
        {
            // OSRM route API: /route/v1/driving/{fromLng,fromLat;toLng,toLat}?overview=full&geometries=polyline
            string coords = $"{fromLng.ToString(System.Globalization.CultureInfo.InvariantCulture)},{fromLat.ToString(System.Globalization.CultureInfo.InvariantCulture)};" +
                            $"{toLng.ToString(System.Globalization.CultureInfo.InvariantCulture)},{toLat.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            var url = $"{_baseUrl}/route/v1/driving/{coords}?overview=full&geometries=polyline";

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            using var resp = await _http.SendAsync(req);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadFromJsonAsync<RouteResponse>();
            var poly = json?.routes?.FirstOrDefault()?.geometry;
            return poly ?? string.Empty;
        }

        // ---- DTOs OSRM ----
        private class TableResponse
        {
            public double?[][]? durations { get; set; }
        }

        private class RouteResponse
        {
            public List<RouteItem>? routes { get; set; }
        }

        private class RouteItem
        {
            public string? geometry { get; set; }
            public double duration { get; set; }
            public double distance { get; set; }
        }
    }
}
