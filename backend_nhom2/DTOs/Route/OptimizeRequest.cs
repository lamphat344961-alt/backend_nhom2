namespace backend_nhom2.DTOs.Route
{
    public record OptimizeRequest(
            int vehicleSpeedKph,
            long departureEpoch,
            List<OptimizePoint> points
        );

    public record OptimizePoint(
        int id,
        string name,
        double lat,
        double lng,
        long? windowStart,
        long? windowEnd,
        int serviceMinutes
    );
}
