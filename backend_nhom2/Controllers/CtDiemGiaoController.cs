using backend_nhom2.Data;
using backend_nhom2.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend_nhom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CtDiemGiaoController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CtDiemGiaoController(AppDbContext db) => _db = db;

        // Upsert: gán điểm giao cho đơn + ràng buộc thời gian (KHÔNG còn BS_XE/ Xe)
        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Upsert([FromBody] CtDiemGiao input, CancellationToken ct)
        {
            // Validate FK
            if (!await _db.DonHangs.AnyAsync(x => x.MADON == input.MADON, ct))
                return BadRequest($"Đơn hàng '{input.MADON}' không tồn tại.");
            if (!await _db.DiemGiaos.AnyAsync(x => x.IdDD == input.IdDD, ct))
                return BadRequest($"Điểm giao '{input.IdDD}' không tồn tại.");

            var entity = await _db.CtDiemGiaos.FindAsync(new object?[] { input.MADON, input.IdDD }, ct);

            if (entity is null)
            {
                entity = new CtDiemGiao
                {
                    MADON = input.MADON,
                    IdDD = input.IdDD,
                    NGAYGIAO = input.NGAYGIAO,
                    TRANGTHAI = string.IsNullOrWhiteSpace(input.TRANGTHAI) ? "CHO_GIAO" : input.TRANGTHAI!,
                    WindowStart = input.WindowStart,
                    WindowEnd = input.WindowEnd,
                    ServiceMinutes = input.ServiceMinutes is > 0 ? input.ServiceMinutes : 10
                };
                _db.CtDiemGiaos.Add(entity);
            }
            else
            {
                entity.NGAYGIAO = input.NGAYGIAO ?? entity.NGAYGIAO;
                entity.TRANGTHAI = string.IsNullOrWhiteSpace(input.TRANGTHAI) ? entity.TRANGTHAI : input.TRANGTHAI!;
                entity.WindowStart = input.WindowStart;
                entity.WindowEnd = input.WindowEnd;
                entity.ServiceMinutes = input.ServiceMinutes is > 0 ? input.ServiceMinutes : (entity.ServiceMinutes ?? 10);
            }

            await _db.SaveChangesAsync(ct);
            return Ok(new { message = "Gán/cập nhật điểm giao cho đơn hàng thành công!", entity });
        }

        // Driver đánh dấu hoàn thành một điểm của đơn
        // QUYỀN dựa trên ĐƠN.HAS XE -> Xe.UserId == userId hiện tại
        [HttpPost("complete")]
        [Authorize(Roles = "Driver")]
        public async Task<IActionResult> Complete([FromQuery] string madon, [FromQuery] string idDD, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized("Token không hợp lệ.");

            // Lấy đơn hàng và xe gán cho đơn
            var don = await _db.DonHangs.AsNoTracking().FirstOrDefaultAsync(x => x.MADON == madon, ct);
            if (don == null) return NotFound("Không tìm thấy đơn hàng.");
            if (string.IsNullOrWhiteSpace(don.BS_XE))
                return Forbid("Đơn hàng chưa được gán xe.");

            var isAssigned = await _db.Xes.AnyAsync(v => v.BS_XE == don.BS_XE && v.UserId == userId, ct);
            if (!isAssigned) return Forbid("Bạn không có quyền cập nhật đơn này.");

            var entity = await _db.CtDiemGiaos.FindAsync(new object?[] { madon, idDD }, ct);
            if (entity is null) return NotFound("Không tìm thấy điểm giao của đơn.");

            entity.TRANGTHAI = "HOANTHANH";
            entity.NGAYGIAO = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return Ok(new { message = "Cập nhật trạng thái hoàn thành thành công!" });
        }

        [HttpGet("by-don/{madon}")]
        [Authorize(Roles = "Owner,Driver")]
        public async Task<IActionResult> ListByDon(string madon, CancellationToken ct)
        {
            var list = await _db.CtDiemGiaos
                .Where(x => x.MADON == madon)
                .Include(x => x.DiemGiao)
                .AsNoTracking()
                .ToListAsync(ct);

            if (!list.Any()) return NotFound("Không tìm thấy điểm giao nào cho đơn hàng này.");
            return Ok(list);
        }
    }
}
