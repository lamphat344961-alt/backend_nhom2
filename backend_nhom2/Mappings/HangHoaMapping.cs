using backend_nhom2.Domain;
using backend_nhom2.DTOs.HangHoa;


namespace backend_nhom2.Mappings;

public static class HangHoaMapping
{
    public static HangHoa ToEntity(this HangHoaCreateDto dto) => new()
    {
        MAHH = dto.MAHH,
        TENHH = dto.TENHH,
        SL = dto.SL,
        MALOAI = dto.MALOAI
    };

    public static void Apply(this HangHoa entity, HangHoaUpdateDto dto)
    {
        entity.TENHH = dto.TENHH;
        entity.SL = dto.SL;
        entity.MALOAI = dto.MALOAI;
    }

    public static HangHoaReadDto ToReadDto(this HangHoa h)
        => new(h.MAHH, h.TENHH, h.SL, h.MALOAI);
}
