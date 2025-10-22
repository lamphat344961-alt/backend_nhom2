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

        /// <summary>
        /// Lấy danh sách đơn hàng được gán cho tài xế hiện tại (dựa trên xe).
        /// </summary>
        [HttpGet("my-deliveries")]
        public async Task<IActionResult> GetMyDeliveries()
        {
            // Lấy UserId từ JWT
            var userIdStr = User.FindAll(ClaimTypes.NameIdentifier)
                                .FirstOrDefault(c => int.TryParse(c.Value, out _))
                                ?.Value;

            if (userIdStr == null || !int.TryParse(userIdStr, out var userId))
                return Unauthorized("Token không hợp lệ hoặc không chứa User ID.");

            // Lấy danh sách xe của tài xế
            var myPlates = await _db.Xes
                .Where(x => x.UserId == userId)
                .Select(x => x.BS_XE)
                .ToListAsync();

            if (myPlates.Count == 0)
                return Ok("Bạn chưa được gán xe nào.");

            // Lấy các đơn hàng của các xe đó, chưa hoàn thành
            var deliveries = await _db.DonHangs
                .Include(d => d.DiemGiao)
                .Where(d =>
                    d.TRANGTHAI != "HOANTHANH" &&
                    d.BS_XE != null &&
                    myPlates.Contains(d.BS_XE))
                .Select(d => new
                {
                    MaDonHang = d.MADON,
                    BienSoXe = d.BS_XE,
                    TenDiemGiao = d.DiemGiao != null ? d.DiemGiao.TEN : null,
                    DiaChiGiao = d.DiemGiao != null ? d.DiemGiao.VITRI : null,
                    Lat = d.DiemGiao != null ? d.DiemGiao.Lat : null,
                    Lng = d.DiemGiao != null ? d.DiemGiao.Lng : null,
                    TrangThai = d.TRANGTHAI,
                    NgayGiaoDuKien = d.NGAYGIAO
                })
                .AsNoTracking()
                .ToListAsync();

            if (deliveries.Count == 0)
                return Ok("Không có đơn giao hàng nào đang chờ giao.");

            return Ok(deliveries);
        }

        /// <summary>
        /// Tài xế đánh dấu hoàn thành 1 đơn hàng.
        /// </summary>
        [HttpPost("complete/{maDon}")]
        public async Task<IActionResult> CompleteOrder(string maDon)
        {
            var userIdStr = User.FindAll(ClaimTypes.NameIdentifier)
                                .FirstOrDefault(c => int.TryParse(c.Value, out _))
                                ?.Value;

            if (userIdStr == null || !int.TryParse(userIdStr, out var userId))
                return Unauthorized("Token không hợp lệ hoặc không chứa User ID.");

            var order = await _db.DonHangs
                .Include(d => d.Xe)
                .FirstOrDefaultAsync(d => d.MADON == maDon);

            if (order == null)
                return NotFound("Không tìm thấy đơn hàng.");

            // Kiểm tra tài xế có quyền
            if (order.Xe == null || order.Xe.UserId != userId)
                return Forbid("Bạn không có quyền hoàn thành đơn này.");

            order.TRANGTHAI = "HOANTHANH";
            order.NGAYGIAO = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok("Đã cập nhật trạng thái hoàn thành đơn hàng.");
        }
    }
}
