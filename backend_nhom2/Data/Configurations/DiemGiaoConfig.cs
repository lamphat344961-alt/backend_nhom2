using backend_nhom2.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace backend_nhom2.Data.Configurations;


public class DiemGiaoConfig : IEntityTypeConfiguration<DiemGiao>
{
    public void Configure(EntityTypeBuilder<DiemGiao> builder)
    {
        builder.ToTable("DIEMGIAO");
        builder.HasKey(x => x.IdDD);
        builder.Property(x => x.IdDD).HasColumnName("D_DD").HasMaxLength(20).IsRequired();
        builder.Property(x => x.VITRI).HasMaxLength(200);
        builder.Property(x => x.TEN).HasMaxLength(100);


    }
}