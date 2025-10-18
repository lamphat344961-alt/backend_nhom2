namespace backend_nhom2.DTOs.CtDonHang;

public record CtDonHangCreateDto(
    string MADON,
    string MAHH,
    decimal DONGIA,
    int SL
);
