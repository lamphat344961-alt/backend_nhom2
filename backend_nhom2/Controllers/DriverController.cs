using backend_nhom2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend_nhom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Driver")]
    public class DriverController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DriverController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("my-deliveries")]
        public async Task<IActionResult> GetMyDeliveries()
        {
            var userIdStr = User.FindAll(ClaimTypes.NameIdentifier)
                                .FirstOrDefault(c => int.TryParse(c.Value, out _))
                                ?.Value;

            if (userIdStr == null || !int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized("Token không hợp lệ hoặc không chứa User ID.");
            }

            // Lấy danh sách biển số xe thuộc tài xế hiện tại
            var myPlates = await _db.Xes
                .Where(x => x.UserId == userId)
                .Select(x => x.BS_XE)
                .ToListAsync();

            if (myPlates.Count == 0)
            {
                return Ok("Bạn chưa được gán xe nào.");
            }

            // Lấy các CtDiemGiao của những Đơn có BS_XE thuộc myPlates và chưa hoàn thành
            var deliveries = await _db.CtDiemGiaos
                .Include(cdg => cdg.DiemGiao)
                .Include(cdg => cdg.DonHang)
                .Where(cdg =>
                    cdg.TRANGTHAI != "HOANTHANH" &&
                    _db.DonHangs.Any(d => d.MADON == cdg.MADON && d.BS_XE != null && myPlates.Contains(d.BS_XE)))
                .AsNoTracking()
                .Select(cdg => new
                {
                    MaDonHang = cdg.MADON,
                    BienSoXe = cdg.DonHang!.BS_XE,
                    IdDiemGiao = cdg.IdDD,
                    TenDiemGiao = cdg.DiemGiao != null ? cdg.DiemGiao.TEN : null,
                    DiaChiGiao = cdg.DiemGiao != null ? cdg.DiemGiao.VITRI : null,
                    TrangThai = cdg.TRANGTHAI,
                    NgayGiaoDuKien = cdg.NGAYGIAO
                })
                .ToListAsync();

            if (deliveries.Count == 0)
            {
                return Ok("Bạn không có điểm giao hàng nào được gán hoặc đã hoàn thành tất cả.");
            }

            return Ok(deliveries);
        }
    }
}