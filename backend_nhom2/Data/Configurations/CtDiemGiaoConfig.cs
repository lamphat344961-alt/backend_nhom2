using backend_nhom2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace backend_nhom2.Data.Configurations;


public class CtDiemGiaoConfig : IEntityTypeConfiguration<CtDiemGiao>
{
    public void Configure(EntityTypeBuilder<CtDiemGiao> builder)
    {
        builder.ToTable("CT_DIEMGIAO");
        builder.HasKey(x => new { x.IdDD, x.MADON });
        builder.Property(x => x.IdDD).HasColumnName("D_DD").HasMaxLength(20).IsRequired();
        builder.Property(x => x.MADON).HasMaxLength(20).IsRequired();
        builder.Property(x => x.TRANGTHAI).HasMaxLength(30);


        builder.HasOne(x => x.DiemGiao)
        .WithMany(d => d.CtDiemGiaos)
        .HasForeignKey(x => x.IdDD)
        .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(x => x.DonHang)
        .WithMany(d => d.CtDiemGiaos)
        .HasForeignKey(x => x.MADON)
        .OnDelete(DeleteBehavior.Cascade);
    }
}