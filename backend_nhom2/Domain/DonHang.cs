namespace backend_nhom2.Domain;


public class DonHang
{
    public string MADON { get; set; } = string.Empty; // PK


    public string? MALOAI { get; set; } // theo schema bạn đưa
    public LoaiHang? LoaiHang { get; set; }


    public DateTime NGAYLAP { get; set; }
    public decimal TONGTIEN { get; set; }


    public ICollection<CtDonHang> CtDonHangs { get; set; } = new List<CtDonHang>();
    public ICollection<CtDiemGiao> CtDiemGiaos { get; set; } = new List<CtDiemGiao>();
}