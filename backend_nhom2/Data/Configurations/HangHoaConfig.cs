using backend_nhom2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace backend_nhom2.Data.Configurations;


public class HangHoaConfig : IEntityTypeConfiguration<HangHoa>
{
    public void Configure(EntityTypeBuilder<HangHoa> builder)
    {
        builder.ToTable("HANGHOA");
        builder.HasKey(x => x.MAHH);
        builder.Property(x => x.MAHH).HasMaxLength(20).IsRequired();
        builder.Property(x => x.TENHH).HasMaxLength(150);
        builder.Property(x => x.SL).IsRequired();


        builder.HasOne(x => x.LoaiHang)
        .WithMany(l => l.HangHoas)
        .HasForeignKey(x => x.MALOAI)
        .OnDelete(DeleteBehavior.Restrict);
    }
}