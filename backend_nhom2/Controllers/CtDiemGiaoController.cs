using backend_nhom2.Data;
using backend_nhom2.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace backend_nhom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // (đăng nhập)
    public class CtDiemGiaoController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CtDiemGiaoController(AppDbContext db)
        {
            _db = db;
        }

        // Endpoint để Owner gán (điểm giao, xe) cho một đơn hàng.
        [HttpPost]
        [Authorize(Roles = "Owner")] // Chỉ có Owner mới được quyền thực hiện hành động này
        public async Task<IActionResult> Upsert(CtDiemGiao input)
        {
            // Tìm kiếm bản ghi hiện có dựa trên khóa chính composite
            var entity = await _db.CtDiemGiaos.FindAsync(input.IdDD, input.BS_XE, input.MADON);

            if (entity is null)
            {
                // Nếu chưa tồn tại, thêm mới
                _db.CtDiemGiaos.Add(input);
            }
            else
            {
                // Nếu đã tồn tại, cập nhật các trường cần thiết
                entity.NGAYGIAO = input.NGAYGIAO;
                entity.TRANGTHAI = input.TRANGTHAI;
            }

            await _db.SaveChangesAsync();
            return Ok(new { message = "Gán điểm giao thành công!" });
        }

        // Endpoint để Tài xế đánh dấu một điểm giao đã hoàn thành.
        [HttpPost("complete")]
        [Authorize(Roles = "Driver")] // Chỉ có Driver mới được quyền thực hiện hành động này
        public async Task<IActionResult> Complete([FromQuery] string d_dd, [FromQuery] string bs_xe, [FromQuery] string madon)
        {
            // Lấy ID của người dùng (tài xế) đang đăng nhập từ token
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
            {
                return Unauthorized("Token không hợp lệ.");
            }

            // KIỂM TRA BẢO MẬT: Đảm bảo tài xế này được quyền lái chiếc xe được gán cho điểm giao.
            var isAssignedToVehicle = await _db.Xes.AnyAsync(v => v.BS_XE == bs_xe && v.UserId == userId);
            if (!isAssignedToVehicle)
            {
                // Nếu tài xế không được gán cho xe này, từ chối quyền truy cập.
                return Forbid("Bạn không có quyền cập nhật trạng thái cho xe này.");
            }

            // Tìm điểm giao cụ thể cần cập nhật
            var entity = await _db.CtDiemGiaos.FindAsync(d_dd, bs_xe, madon);
            if (entity is null)
            {
                return NotFound("Không tìm thấy điểm giao được chỉ định.");
            }

            // Cập nhật trạng thái và ngày giao
            entity.TRANGTHAI = "HOANTHANH";
            entity.NGAYGIAO = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái hoàn thành thành công!" });
        }

        // Endpoint để lấy danh sách các điểm giao theo mã đơn hàng.
        [HttpGet("by-don/{madon}")]
        [Authorize(Roles = "Owner,Driver")] // CHỈ ĐỊNH: Cả Owner và Driver đều có thể xem thông tin này
        public async Task<IActionResult> ListByDon(string madon)
        {
            var list = await _db.CtDiemGiaos
                .Where(x => x.MADON == madon)
                .Include(x => x.DiemGiao) // Nạp thông tin chi tiết của Điểm Giao
                .Include(x => x.Xe)       // Nạp thông tin chi tiết của Xe
                .AsNoTracking()
                .ToListAsync();

            if (!list.Any())
            {
                return NotFound("Không tìm thấy điểm giao nào cho đơn hàng này.");
            }

            return Ok(list);
        }
    }
}