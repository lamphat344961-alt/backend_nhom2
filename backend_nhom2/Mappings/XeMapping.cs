using backend_nhom2.Domain;
using backend_nhom2.Dtos.Xe;

namespace backend_nhom2.Mappings;

public static class XeMapping
{
    public static void Apply(this Xe entity, XeUpdateDto dto)
    {
        if (dto.TT_XE is not null) entity.TT_XE = dto.TT_XE;
        if (dto.TenXe is not null) entity.TENXE = dto.TenXe;
    }
}
