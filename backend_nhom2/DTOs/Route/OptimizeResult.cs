namespace backend_nhom2.DTOs.Route
{
    public record OptimizedStop(
         int pointId,
         string name,
         double lat,
         double lng,
         long etaEpoch,
         string etaIso,
         string polyline
     );

    public record OptimizeResult(
        int routeId,
        List<OptimizedStop> stops,
        int totalSeconds,
        string readableTotal
    );
}
