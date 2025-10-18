namespace backend_nhom2.DTOs.HangHoa;

public record HangHoaCreateDto(
    string MAHH,
    string? TENHH,
    int SL,
    string? MALOAI
);
