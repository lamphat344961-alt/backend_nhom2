using backend_nhom2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace backend_nhom2.Data.Configurations;


public class DonHangConfig : IEntityTypeConfiguration<DonHang>
{
    public void Configure(EntityTypeBuilder<DonHang> builder)
    {
        builder.ToTable("DONHANG");
        builder.HasKey(x => x.MADON);
        builder.Property(x => x.MADON).HasMaxLength(20).IsRequired();
        builder.Property(x => x.NGAYLAP).IsRequired();
        builder.Property(x => x.TONGTIEN).HasColumnType("decimal(18,2)").IsRequired();


        builder.HasOne(d => d.LoaiHang)
        .WithMany(l => l.DonHangs)
        .HasForeignKey(d => d.MALOAI)
        .OnDelete(DeleteBehavior.Restrict);


        builder.HasMany(d => d.CtDonHangs)
        .WithOne(cd => cd.DonHang!)
        .HasForeignKey(cd => cd.MADON)
        .OnDelete(DeleteBehavior.Cascade);


        builder.HasMany(d => d.CtDiemGiaos)
        .WithOne(cg => cg.DonHang!)
        .HasForeignKey(cg => cg.MADON)
        .OnDelete(DeleteBehavior.Cascade);
    }
}