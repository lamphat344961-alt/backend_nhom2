using backend_nhom2.Domain;
using Microsoft.EntityFrameworkCore;


namespace backend_nhom2.Data;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


    public DbSet<Xe> Xes => Set<Xe>();
    public DbSet<DonHang> DonHangs => Set<DonHang>();
    public DbSet<DiemGiao> DiemGiaos => Set<DiemGiao>();
    public DbSet<LoaiHang> LoaiHangs => Set<LoaiHang>();
    public DbSet<HangHoa> HangHoas => Set<HangHoa>();
    public DbSet<CtDonHang> CtDonHangs => Set<CtDonHang>();
    public DbSet<CtDiemGiao> CtDiemGiaos => Set<CtDiemGiao>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);


        // ====== Seed Data (demo) ======
        modelBuilder.Entity<LoaiHang>().HasData(
        new LoaiHang { MALOAI = "L1", TENLOAI = "Điện tử", SL = 0 },
        new LoaiHang { MALOAI = "L2", TENLOAI = "Thời trang", SL = 0 }
        );


        modelBuilder.Entity<HangHoa>().HasData(
        new HangHoa { MAHH = "H001", TENHH = "Tai nghe", SL = 100, MALOAI = "L1" },
        new HangHoa { MAHH = "H002", TENHH = "Áo thun", SL = 200, MALOAI = "L2" }
        );


        modelBuilder.Entity<Xe>().HasData(
        new Xe { BS_XE = "51A-00001", TENXE = "Xe Tải Nhỏ", TT_XE = "Sẵn sàng" },
        new Xe { BS_XE = "51B-00002", TENXE = "Xe Bán Tải", TT_XE = "Bảo dưỡng" }
        );


        modelBuilder.Entity<DiemGiao>().HasData(
        new DiemGiao { IdDD = "DG01", TEN = "Kho Quận 1", VITRI = "Q1" },
        new DiemGiao { IdDD = "DG02", TEN = "Kho Quận 7", VITRI = "Q7" }
        );


        modelBuilder.Entity<DonHang>().HasData(
        new DonHang { MADON = "DH001", MALOAI = "L1", NGAYLAP = new DateTime(2025, 10, 16), TONGTIEN = 0M },
        new DonHang { MADON = "DH002", MALOAI = "L2", NGAYLAP = new DateTime(2025, 10, 16), TONGTIEN = 0M }
        );


        modelBuilder.Entity<CtDonHang>().HasData(
        new { MAHH = "H001", MADON = "DH001", DONGIA = 350000m, SL = 2 },
        new { MAHH = "H002", MADON = "DH001", DONGIA = 120000m, SL = 3 },
        new { MAHH = "H002", MADON = "DH002", DONGIA = 110000m, SL = 1 }
        );


        modelBuilder.Entity<CtDiemGiao>().HasData(
        new { IdDD = "DG01", BS_XE = "51A-00001", MADON = "DH001", NGAYGIAO = (DateTime?)null, TRANGTHAI = "CHO_GIAO" },
        new { IdDD = "DG02", BS_XE = "51B-00002", MADON = "DH002", NGAYGIAO = (DateTime?)null, TRANGTHAI = "CHO_GIAO" }
        );
    }
}