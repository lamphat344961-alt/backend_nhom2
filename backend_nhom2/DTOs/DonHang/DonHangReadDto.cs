namespace backend_nhom2.DTOs.DonHang;

public record DonHangReadDto(
    string MADON,
    string? MALOAI,
    DateTime NGAYLAP,
    decimal TONGTIEN
);
