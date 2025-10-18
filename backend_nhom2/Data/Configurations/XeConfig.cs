using backend_nhom2.Domain;
using backend_nhom2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend_nhom2.Data.Configurations
{
    public class XeConfig : IEntityTypeConfiguration<Xe>
    {
        public void Configure(EntityTypeBuilder<Xe> builder)
        {
            builder.ToTable("XE");
            builder.HasKey(x => x.BS_XE);
            builder.Property(x => x.BS_XE).HasMaxLength(20).IsRequired();
            builder.Property(x => x.TENXE).HasMaxLength(100);
            builder.Property(x => x.TT_XE).HasMaxLength(50);

            builder.HasMany(x => x.CtDiemGiaos)
            .WithOne(cd => cd.Xe!)
            .HasForeignKey(cd => cd.BS_XE)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}