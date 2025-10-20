namespace backend_nhom2.DTOs.Route
{
    public class OptimizeFromOrdersRequest

    {
        public List<string> orderIds { get; set; } = new(); // MADON
        public int vehicleSpeedKph { get; set; } = 40;      // hiện tại chưa dùng vào cost (OSRM trả duration)
        public long departureEpoch { get; set; }
    }
}
