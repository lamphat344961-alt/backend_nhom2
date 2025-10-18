namespace backend_nhom2.DTOs.HangHoa;

public record HangHoaReadDto(
    string MAHH,
    string? TENHH,
    int SL,
    string? MALOAI
);
