namespace backend_nhom2.Dtos.DonHang;

public record DonHangReadDto(
    string MADON,
    string? MALOAI,
    DateTime NGAYLAP,
    decimal TONGTIEN
);
