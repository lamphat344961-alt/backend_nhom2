namespace backend_nhom2.DTOs
{

    public class DonHangItemReadDto
    {
        public string MAHH { get; set; } = string.Empty;
        public string? TENHH { get; set; }
        public int SL { get; set; }
        public decimal DONGIA { get; set; }
    }


    public class DonHangReadDto
    {
        public string MADON { get; set; } = string.Empty;
        public DateTime NGAYLAP { get; set; }
        public decimal TONGTIEN { get; set; }
        public string? MALOAI { get; set; }
        public List<DonHangItemReadDto> Items { get; set; } = new();
    }
}