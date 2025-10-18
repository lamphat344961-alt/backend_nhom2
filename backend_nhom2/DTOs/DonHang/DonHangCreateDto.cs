namespace backend_nhom2.DTOs.DonHang;

public record DonHangCreateDto(
    string MADON,
    string? MALOAI,
    DateTime NGAYLAP,
    decimal TONGTIEN
);
