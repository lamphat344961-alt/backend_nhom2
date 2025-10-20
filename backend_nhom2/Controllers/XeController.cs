using backend_nhom2.Data;
using backend_nhom2.Domain;
using backend_nhom2.DTOs.Xe;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_nhom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Owner")] // Chỉ Owner được quản lý xe
    public class XeController : ControllerBase
    {
        private readonly AppDbContext _db;
        public XeController(AppDbContext db) => _db = db;

        // GET: api/Xe - Lấy danh sách xe kèm thông tin tài xế
        [HttpGet]
        public async Task<ActionResult<IEnumerable<XeReadDto>>> GetAll()
        {
            var xes = await _db.Xes
                .Include(x => x.User) // Nạp thông tin User (tài xế)
                .AsNoTracking()
                .Select(x => new XeReadDto
                {
                    BS_XE = x.BS_XE,
                    TENXE = x.TENXE,
                    TT_XE = x.TT_XE,
                    UserId = x.UserId,
                    DriverFullName = x.User != null ? x.User.FullName : null
                })
                .ToListAsync();
            return Ok(xes);
        }

        // GET: api/Xe/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<XeReadDto>> GetById(string id)
        {
            var xe = await _db.Xes.Include(x => x.User).FirstOrDefaultAsync(x => x.BS_XE == id);
            if (xe is null) return NotFound();

            var dto = new XeReadDto
            {
                BS_XE = xe.BS_XE,
                TENXE = xe.TENXE,
                TT_XE = xe.TT_XE,
                UserId = xe.UserId,
                DriverFullName = xe.User != null ? xe.User.FullName : null
            };
            return Ok(dto);
        }

        // POST: api/Xe
        [HttpPost]
        public async Task<ActionResult<XeReadDto>> Create(XeCreateDto dto)
        {
            if (await _db.Xes.AnyAsync(x => x.BS_XE == dto.BS_XE))
            {
                return Conflict($"Biển số xe '{dto.BS_XE}' đã tồn tại.");
            }

            var xe = new Xe
            {
                BS_XE = dto.BS_XE,
                TENXE = dto.TENXE,
                TT_XE = dto.TT_XE
            };

            _db.Xes.Add(xe);
            await _db.SaveChangesAsync();

            var readDto = new XeReadDto { BS_XE = xe.BS_XE, TENXE = xe.TENXE, TT_XE = xe.TT_XE };
            return CreatedAtAction(nameof(GetById), new { id = xe.BS_XE }, readDto);
        }

        // PUT: api/Xe/{bsxe} - Cập nhật thông tin cơ bản của xe
        [HttpPut("{bsxe}")]
        public async Task<IActionResult> Update(string bsxe, [FromBody] XeUpdateDto dto)
        {
            var xe = await _db.Xes.FindAsync(bsxe);
            if (xe is null) return NotFound();

            xe.TENXE = dto.TenXe; // thuộc tính theo DTO của bạn
            xe.TT_XE = dto.TT_XE;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/Xe/{bsxe}/assign-driver - Gán tài xế cho xe
        [HttpPut("{bsxe}/assign-driver")]
        public async Task<IActionResult> AssignDriver(string bsxe, [FromBody] AssignDriverDto dto)
        {
            var xe = await _db.Xes.FindAsync(bsxe);
            if (xe is null) return NotFound($"Không tìm thấy xe với biển số: {bsxe}");

            var driver = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == dto.UserId);

            if (driver is null) return BadRequest($"Không tìm thấy tài xế với ID: {dto.UserId}");
            if (driver.Role?.RoleName != "Driver") return BadRequest($"Người dùng có ID {dto.UserId} không phải là tài xế.");

            xe.UserId = dto.UserId;
            await _db.SaveChangesAsync();

            return Ok(new { message = $"Đã gán tài xế '{driver.FullName}' cho xe '{bsxe}' thành công." });
        }

        // DELETE: api/Xe/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var xe = await _db.Xes.FindAsync(id);
            if (xe is null) return NotFound();

            // Quan hệ mới: Xe 1-N DonHang
            // => Không cho xóa nếu có đơn đang gán xe này
            bool inUse = await _db.DonHangs.AnyAsync(d => d.BS_XE == id);
            if (inUse) return Conflict("Không thể xóa xe này vì đang được gán cho một hoặc nhiều đơn hàng.");

            _db.Xes.Remove(xe);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
