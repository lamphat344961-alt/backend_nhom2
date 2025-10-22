namespace backend_nhom2.Domain;


public class DonHang
{
    public string MADON { get; set; } = string.Empty; // PK


    public string? MALOAI { get; set; } // theo schema bạn đưa
    public LoaiHang? LoaiHang { get; set; }


    public DateTime NGAYLAP { get; set; }
    public decimal TONGTIEN { get; set; }

    public string? BS_XE { get; set; }
    public Xe? Xe { get; set; }

    public string? D_DD { get; set; }            // nullable nếu cho phép đơn chưa gắn điểm giao
    public DiemGiao? DiemGiao { get; set; }

    public DateTime? NGAYGIAO { get; set; }
    public string TRANGTHAI { get; set; } = "CHO_GIAO";
    public long? WindowStart { get; set; }
    public long? WindowEnd { get; set; }
    public int? ServiceMinutes { get; set; } = 10;

    public ICollection<CtDonHang> CtDonHangs { get; set; } = new List<CtDonHang>();

}