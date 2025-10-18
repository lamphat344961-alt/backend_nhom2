using backend_nhom2.Data;
using backend_nhom2.Domain;
using backend_nhom2.DTOs.HangHoa;
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
    [Authorize(Roles = "Owner")] // Chỉ Owner được quản lý hàng hóa
    public class HangHoaController : ControllerBase
    {
        private readonly AppDbContext _db;
        public HangHoaController(AppDbContext db) => _db = db;

        // GET: api/HangHoa
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HangHoaReadDto>>> GetAll()
        {
            var hangHoas = await _db.HangHoas
                .AsNoTracking()

                .Select(h => new HangHoaReadDto(h.MAHH, h.TENHH, h.SL, h.MALOAI))
                .ToListAsync();
            return Ok(hangHoas);
        }

        // GET: api/HangHoa/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<HangHoaReadDto>> GetById(string id)
        {
            var hh = await _db.HangHoas.AsNoTracking().FirstOrDefaultAsync(x => x.MAHH == id);
            if (hh is null)
            {
                return NotFound($"Không tìm thấy hàng hóa với mã: {id}");
            }

            return Ok(new HangHoaReadDto(hh.MAHH, hh.TENHH, hh.SL, hh.MALOAI));
        }

        // POST: api/HangHoa
        [HttpPost]
        public async Task<ActionResult<HangHoaReadDto>> Create([FromBody] HangHoaCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MAHH)) return BadRequest("MAHH là bắt buộc.");
            if (await _db.HangHoas.AnyAsync(x => x.MAHH == dto.MAHH)) return Conflict($"MAHH '{dto.MAHH}' đã tồn tại.");

            // Thêm validation cho khóa ngoại MALOAI
            if (!string.IsNullOrEmpty(dto.MALOAI) && !await _db.LoaiHangs.AnyAsync(l => l.MALOAI == dto.MALOAI))
            {
                return BadRequest($"Mã loại hàng '{dto.MALOAI}' không tồn tại.");
            }

            var entity = new HangHoa
            {
                MAHH = dto.MAHH,
                TENHH = dto.TENHH,
                SL = dto.SL,
                MALOAI = dto.MALOAI
            };

            _db.HangHoas.Add(entity);
            await _db.SaveChangesAsync();

            var readDto = new HangHoaReadDto(entity.MAHH, entity.TENHH, entity.SL, entity.MALOAI);
            return CreatedAtAction(nameof(GetById), new { id = entity.MAHH }, readDto);
        }

        // PUT: api/HangHoa/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] HangHoaUpdateDto dto)
        {
            var entity = await _db.HangHoas.FirstOrDefaultAsync(x => x.MAHH == id);
            if (entity is null) return NotFound();

            // Thêm validation cho khóa ngoại MALOAI
            if (!string.IsNullOrEmpty(dto.MALOAI) && !await _db.LoaiHangs.AnyAsync(l => l.MALOAI == dto.MALOAI))
            {
                return BadRequest($"Mã loại hàng '{dto.MALOAI}' không tồn tại.");
            }

            // Ánh xạ thủ công các thay đổi từ DTO
            entity.TENHH = dto.TENHH;
            entity.SL = dto.SL;
            entity.MALOAI = dto.MALOAI;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/HangHoa/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await _db.HangHoas.FirstOrDefaultAsync(x => x.MAHH == id);
            if (entity is null) return NotFound();

            _db.HangHoas.Remove(entity);
            try
            {
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                // Cung cấp thông báo lỗi rõ ràng hơn
                return Conflict("Không thể xóa hàng hóa này vì nó đang được sử dụng trong một đơn hàng.");
            }
        }
    }
}