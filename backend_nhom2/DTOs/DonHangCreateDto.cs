namespace backend_nhom2.DTOs;


public class DonHangCreateItemDto
{
    public string MAHH { get; set; } = string.Empty;
    public int SL { get; set; }
    public decimal DONGIA { get; set; }
}


public class DonHangCreateDto
{
    public string MADON { get; set; } = string.Empty;
    public string? MALOAI { get; set; }
    public DateTime NGAYLAP { get; set; }
    public List<DonHangCreateItemDto> Items { get; set; } = new();
}