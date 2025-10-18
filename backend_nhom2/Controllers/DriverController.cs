using backend_nhom2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized("Token không hợp lệ hoặc không chứa User ID.");
            }

            var deliveries = await _db.CtDiemGiaos
                .Include(cdg => cdg.DiemGiao)
                .Include(cdg => cdg.DonHang)
                .Where(cdg => cdg.Xe != null && cdg.Xe.UserId == userId && cdg.TRANGTHAI != "HOANTHANH")
                .Select(cdg => new
                {
                    MaDonHang = cdg.MADON,
                    IdDiemGiao = cdg.IdDD,

                    TenDiemGiao = cdg.DiemGiao == null ? null : cdg.DiemGiao.TEN,

                    DiaChiGiao = cdg.DiemGiao == null ? null : cdg.DiemGiao.VITRI,

                    TrangThai = cdg.TRANGTHAI,
                    BienSoXe = cdg.BS_XE,
                    NgayGiaoDuKien = cdg.NGAYGIAO
                })
                .AsNoTracking()
                .ToListAsync();

            if (!deliveries.Any())
            {
                return Ok("Bạn không có điểm giao hàng nào được gán hoặc đã hoàn thành tất cả.");
            }

            return Ok(deliveries);
        }
    }
}