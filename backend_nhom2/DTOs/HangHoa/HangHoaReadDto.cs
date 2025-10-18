// path: backend_nhom2/Dtos/HangHoa/HangHoaReadDto.cs
namespace backend_nhom2.Dtos.HangHoa;

public record HangHoaReadDto(
    string MAHH,
    string? TENHH,
    int SL,
    string? MALOAI
);
