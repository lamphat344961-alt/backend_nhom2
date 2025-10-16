namespace backend_nhom2.Domain;


public class CtDiemGiao
{
    // PK ghép: IdDD + BS_XE + MADON
    public string IdDD { get; set; } = string.Empty; // maps to column D_DD
    public string BS_XE { get; set; } = string.Empty;
    public string MADON { get; set; } = string.Empty;


    public DateTime? NGAYGIAO { get; set; }
    public string? TRANGTHAI { get; set; }


    public DiemGiao? DiemGiao { get; set; }
    public Xe? Xe { get; set; }
    public DonHang? DonHang { get; set; }
}