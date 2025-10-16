namespace backend_nhom2.Domain;


public class CtDonHang
{
    // PK ghép: MAHH + MADON
    public string MAHH { get; set; } = string.Empty;
    public string MADON { get; set; } = string.Empty;


    public decimal DONGIA { get; set; }
    public int SL { get; set; }


    public HangHoa? HangHoa { get; set; }
    public DonHang? DonHang { get; set; }
}