using System.ComponentModel.DataAnnotations;

namespace backend_nhom2.Models.Route
{
    public class RouteStop
    {
        public int Id { get; set; }

        public int RoutePlanId { get; set; }
        public RoutePlan RoutePlan { get; set; } = default!;

        public int Order { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;

        public double Lat { get; set; }
        public double Lng { get; set; }

        public long EtaEpoch { get; set; }

        [Required, MaxLength(40)]
        public string EtaIso { get; set; } = default!;

        [MaxLength(4000)]
        public string? Polyline { get; set; }
    }
}
