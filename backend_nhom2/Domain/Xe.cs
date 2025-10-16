namespace backend_nhom2.Domain;


public class Xe
{
    // PK
    public string BS_XE { get; set; } = string.Empty; // Biển số


    public string? TT_XE { get; set; } // Trạng thái xe
    public string? TENXE { get; set; }


    // Nav
    public ICollection<CtDiemGiao> CtDiemGiaos { get; set; } = new List<CtDiemGiao>();
}