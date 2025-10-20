using backend_nhom2.Models;

namespace backend_nhom2.Domain;

public class Xe
{
    public string BS_XE { get; set; } = string.Empty; // Biển số

    public string? TT_XE { get; set; } // Trạng thái xe
    public string? TENXE { get; set; }


    public int? UserId { get; set; }

    public User? User { get; set; }

    // Nav
    public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}