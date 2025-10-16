using backend_nhom2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace backend_nhom2.Data.Configurations;


public class CtDonHangConfig : IEntityTypeConfiguration<CtDonHang>
{
    public void Configure(EntityTypeBuilder<CtDonHang> builder)
    {
        builder.ToTable("CT_DONHANG");
        builder.HasKey(x => new { x.MAHH, x.MADON });
        builder.Property(x => x.DONGIA).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(x => x.SL).IsRequired();


        builder.HasOne(x => x.HangHoa)
        .WithMany(h => h.CtDonHangs)
        .HasForeignKey(x => x.MAHH)
        .OnDelete(DeleteBehavior.Restrict);
    }
}