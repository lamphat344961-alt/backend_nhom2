namespace backend_nhom2.Dtos.DonHang;

public record DonHangCreateDto(
    string MADON,
    string? MALOAI,
    DateTime NGAYLAP,
    decimal TONGTIEN
);
