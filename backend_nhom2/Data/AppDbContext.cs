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

            
        }
    }
}
