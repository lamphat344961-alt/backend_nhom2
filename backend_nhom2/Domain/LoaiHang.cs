namespace backend_nhom2.Domain;


public class LoaiHang
{
    public string MALOAI { get; set; } = string.Empty; // PK
    public string? TENLOAI { get; set; }
    public int? SL { get; set; } // tuỳ nghiệp vụ (tổng số loại hiện có)


    public ICollection<HangHoa> HangHoas { get; set; } = new List<HangHoa>();
    public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}