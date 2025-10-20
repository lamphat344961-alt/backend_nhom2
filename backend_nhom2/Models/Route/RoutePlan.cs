using System.ComponentModel.DataAnnotations;

namespace backend_nhom2.Models.Route
{
    public class RoutePlan
    {
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Note { get; set; }

        public int TotalSeconds { get; set; }

        public ICollection<RouteStop> Stops { get; set; } = new List<RouteStop>();
    }
}
