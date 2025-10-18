using backend_nhom2.Data;
using backend_nhom2.Domain;
using backend_nhom2.DTOs.CtDonHang;
using backend_nhom2.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_nhom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CtDonHangController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CtDonHangController(AppDbContext db) => _db = db;

        // GET: api/CtDonHang?madon=...&mahh=...
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CtDonHangReadDto>>> GetAll([FromQuery] string? madon, [FromQuery] string? mahh)
        {
            var query = _db.CtDonHangs.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(madon))
            {
                query = query.Where(x => x.MADON == madon);
            }
            if (!string.IsNullOrWhiteSpace(mahh))
            {
                query = query.Where(x => x.MAHH == mahh);
            }
            var data = await query.Select(x => x.ToReadDto()).ToListAsync();
            return Ok(data);
        }

        // GET: api/CtDonHang/{madon}/{mahh}
        [HttpGet("{madon}/{mahh}")]
        public async Task<ActionResult<CtDonHangReadDto>> GetById(string madon, string mahh)
        {
            var entity = await _db.CtDonHangs.AsNoTracking()
                .FirstOrDefaultAsync(x => x.MADON == madon && x.MAHH == mahh);
            return entity is null ? NotFound() : Ok(entity.ToReadDto());
        }

        // POST: api/CtDonHang
        [HttpPost]
        public async Task<ActionResult<CtDonHangReadDto>> Create([FromBody] CtDonHangCreateDto dto)
        {
            if (dto.SL <= 0) return BadRequest("Số lượng (SL) phải lớn hơn 0.");
            if (dto.DONGIA < 0) return BadRequest("Đơn giá (DONGIA) không được âm.");

            var exists = await _db.CtDonHangs.AnyAsync(x => x.MADON == dto.MADON && x.MAHH == dto.MAHH);
            if (exists) return Conflict($"Chi tiết đơn hàng cho mặt hàng '{dto.MAHH}' đã tồn tại trong đơn '{dto.MADON}'.");

            var donExists = await _db.DonHangs.AnyAsync(d => d.MADON == dto.MADON);
            if (!donExists) return BadRequest($"Đơn hàng với mã '{dto.MADON}' không tồn tại.");

            var hhExists = await _db.HangHoas.AnyAsync(h => h.MAHH == dto.MAHH);
            if (!hhExists) return BadRequest($"Hàng hóa với mã '{dto.MAHH}' không tồn tại.");

            var entity = dto.ToEntity();
            _db.CtDonHangs.Add(entity);
            await _db.SaveChangesAsync();

            // *** TÍCH HỢP LOGIC: Cập nhật tổng tiền của đơn hàng ***
            await UpdateDonHangTongTienAsync(entity.MADON);

            var readDto = entity.ToReadDto();
            return CreatedAtAction(nameof(GetById), new { madon = entity.MADON, mahh = entity.MAHH }, readDto);
        }

        // PUT: api/CtDonHang/{madon}/{mahh}
        [HttpPut("{madon}/{mahh}")]
        public async Task<IActionResult> Update(string madon, string mahh, [FromBody] CtDonHangUpdateDto dto)
        {
            if (dto.SL <= 0) return BadRequest("Số lượng (SL) phải lớn hơn 0.");
            if (dto.DONGIA < 0) return BadRequest("Đơn giá (DONGIA) không được âm.");

            var entity = await _db.CtDonHangs.FirstOrDefaultAsync(x => x.MADON == madon && x.MAHH == mahh);
            if (entity is null) return NotFound();

            entity.Apply(dto);
            await _db.SaveChangesAsync();

            // *** TÍCH HỢP LOGIC: Cập nhật lại tổng tiền của đơn hàng sau khi sửa ***
            await UpdateDonHangTongTienAsync(madon);

            return NoContent();
        }

        // DELETE: api/CtDonHang/{madon}/{mahh}
        [HttpDelete("{madon}/{mahh}")]
        public async Task<IActionResult> Delete(string madon, string mahh)
        {
            var entity = await _db.CtDonHangs.FirstOrDefaultAsync(x => x.MADON == madon && x.MAHH == mahh);
            if (entity is null) return NotFound();

            _db.CtDonHangs.Remove(entity);
            await _db.SaveChangesAsync();

            // *** TÍCH HỢP LOGIC: Cập nhật lại tổng tiền của đơn hàng sau khi xóa ***
            await UpdateDonHangTongTienAsync(madon);

            return NoContent();
        }

        // --- PHƯƠNG THỨC HELPER ĐỂ CẬP NHẬT TỔNG TIỀN ---
        private async Task UpdateDonHangTongTienAsync(string madon)
        {
            var donHang = await _db.DonHangs.FindAsync(madon);
            if (donHang != null)
            {
                // Tính toán lại tổng tiền bằng cách SUM tất cả các chi tiết đơn hàng liên quan
                donHang.TONGTIEN = await _db.CtDonHangs
                    .Where(ct => ct.MADON == madon)
                    .SumAsync(ct => ct.SL * ct.DONGIA);

                // Lưu thay đổi tổng tiền vào CSDL
                _db.DonHangs.Update(donHang);
                await _db.SaveChangesAsync();
            }
        }
    }
}