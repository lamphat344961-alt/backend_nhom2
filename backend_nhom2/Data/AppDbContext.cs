using backend_nhom2.Domain;
using backend_nhom2.Models;
using backend_nhom2.Models.Route;
using Microsoft.EntityFrameworkCore;

namespace backend_nhom2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ====== Các bảng nghiệp vụ ======
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Xe> Xes => Set<Xe>();
        public DbSet<DonHang> DonHangs => Set<DonHang>();
        public DbSet<DiemGiao> DiemGiaos => Set<DiemGiao>();
        public DbSet<LoaiHang> LoaiHangs => Set<LoaiHang>();
        public DbSet<HangHoa> HangHoas => Set<HangHoa>();
        public DbSet<CtDonHang> CtDonHangs => Set<CtDonHang>();
        public DbSet<CtDiemGiao> CtDiemGiaos => Set<CtDiemGiao>();

        // ====== Các bảng tối ưu lộ trình ======
        public DbSet<RoutePlan> RoutePlans => Set<RoutePlan>();
        public DbSet<RouteStop> RouteStops => Set<RouteStop>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ====== Apply Configurations từ assembly ======
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // ==========================
            // PHẦN CẤU HÌNH LỘ TRÌNH (RoutePlan / RouteStop)
            // ==========================
            modelBuilder.Entity<RoutePlan>(e =>
            {
                e.ToTable("RoutePlans");
                e.HasKey(x => x.Id);
                e.Property(x => x.CreatedAt).HasColumnType("datetime2");
                e.Property(x => x.Note).HasMaxLength(500);
                e.HasMany(x => x.Stops)
                    .WithOne(x => x.RoutePlan)
                    .HasForeignKey(x => x.RoutePlanId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RouteStop>(e =>
            {
                e.ToTable("RouteStops");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(200);
                e.Property(x => x.EtaIso).IsRequired().HasMaxLength(40);
                e.Property(x => x.Polyline).HasMaxLength(4000);
                e.HasIndex(x => new { x.RoutePlanId, x.Order }).IsUnique();
            });

            modelBuilder.Entity<CtDiemGiao>(e =>
            {
                e.ToTable("CT_DIEMGIAO");
                // PK phức hợp: MADON + D_DD
                e.HasKey(x => new { x.MADON, x.IdDD });

                e.Property(x => x.MADON).HasMaxLength(20);
                e.Property(x => x.IdDD).HasColumnName("D_DD").HasMaxLength(20);
                e.Property(x => x.TRANGTHAI).HasMaxLength(30).IsRequired();
                e.Property(x => x.ServiceMinutes).HasDefaultValue(10);

                e.HasOne(x => x.DonHang)
                    .WithMany(d => d.CtDiemGiaos)
                    .HasForeignKey(x => x.MADON)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.DiemGiao)
                    .WithMany(d => d.CtDiemGiaos)
                    .HasForeignKey(x => x.IdDD)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Xe>(e =>
            {
                e.ToTable("XE");
                e.HasKey(x => x.BS_XE);
                e.Property(x => x.BS_XE).HasMaxLength(20);
                e.Property(x => x.TENXE).HasMaxLength(100);
                e.Property(x => x.TT_XE).HasMaxLength(50);
            });
            // ===== DONHANG (mỗi đơn gán 1 xe) =====
            modelBuilder.Entity<DonHang>(e =>
            {
                e.ToTable("DONHANG");
                e.HasKey(x => x.MADON);
                e.Property(x => x.MADON).HasMaxLength(20);

                e.Property(x => x.MALOAI).HasMaxLength(20);
                e.Property(x => x.TONGTIEN).HasColumnType("decimal(18,2)");

                // FK đến Xe (nullable), OnDelete SetNull
                e.Property(x => x.BS_XE).HasMaxLength(20).IsRequired(false);
                e.HasOne(x => x.Xe)
                    .WithMany(v => v.DonHangs)
                    .HasForeignKey(x => x.BS_XE)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            // ===== DIEMGIAO =====
            modelBuilder.Entity<DiemGiao>(e =>
            {
                e.ToTable("DIEMGIAO");
                e.HasKey(x => x.IdDD);
                e.Property(x => x.IdDD).HasColumnName("D_DD").HasMaxLength(20);

                e.Property(x => x.TEN).HasMaxLength(200);
                e.Property(x => x.VITRI).HasMaxLength(300);
            });
        }
    }
}
