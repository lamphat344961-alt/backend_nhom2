namespace backend_nhom2.DTOs.DonHang;

public record DonHangUpdateDto(
    string? MALOAI,
    DateTime NGAYLAP,
    string? D_DD,
    long? WindowStart = null,
    long? WindowEnd = null,
    int? ServiceMinutes = 10,
    string? TRANGTHAI = "CHO_GIAO"
);
