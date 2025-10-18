using backend_nhom2.Domain;
using backend_nhom2.DTOs.CtDonHang;

namespace backend_nhom2.Mappings;

public static class CtDonHangMapping
{
    public static CtDonHang ToEntity(this CtDonHangCreateDto dto) => new()
    {
        MADON = dto.MADON,
        MAHH = dto.MAHH,
        DONGIA = dto.DONGIA,
        SL = dto.SL
    };

    public static void Apply(this CtDonHang entity, CtDonHangUpdateDto dto)
    {
        entity.DONGIA = dto.DONGIA;
        entity.SL = dto.SL;
    }

    public static CtDonHangReadDto ToReadDto(this CtDonHang e)
        => new(e.MADON, e.MAHH, e.DONGIA, e.SL);
}
