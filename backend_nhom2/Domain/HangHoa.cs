namespace backend_nhom2.Domain;


public class HangHoa
{
    public string MAHH { get; set; } = string.Empty; // PK
    public string? TENHH { get; set; }
    public int SL { get; set; }


    // Optional: gắn loại hàng
    public string? MALOAI { get; set; }
    public LoaiHang? LoaiHang { get; set; }


    public ICollection<CtDonHang> CtDonHangs { get; set; } = new List<CtDonHang>();
}