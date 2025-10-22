namespace backend_nhom2.DTOs.DonHang;

public record DonHangCreateDto(
    string MADON,
    string? MALOAI,
    DateTime NGAYLAP,
    decimal TONGTIEN,
    string? BS_XE,
    string? D_DD,              // NEW: gán điểm giao
    long? WindowStart = null,
    long? WindowEnd = null,
    int? ServiceMinutes = 10,
    string? TRANGTHAI = "CHO_GIAO"
);
