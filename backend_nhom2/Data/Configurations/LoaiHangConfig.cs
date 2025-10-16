using backend_nhom2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace backend_nhom2.Data.Configurations;


public class LoaiHangConfig : IEntityTypeConfiguration<LoaiHang>
{
    public void Configure(EntityTypeBuilder<LoaiHang> builder)
    {
        builder.ToTable("LOAIHANG");
        builder.HasKey(x => x.MALOAI);
        builder.Property(x => x.MALOAI).HasMaxLength(20).IsRequired();
        builder.Property(x => x.TENLOAI).HasMaxLength(100);
    }
}