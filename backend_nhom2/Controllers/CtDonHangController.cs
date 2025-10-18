using backend_nhom2.Data;
using backend_nhom2.Domain;
using backend_nhom2.Dtos.CtDonHang;
using backend_nhom2.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_nhom2.Controllers;

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
        var q = _db.CtDonHangs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(madon)) q = q.Where(x => x.MADON == madon);
        if (!string.IsNullOrWhiteSpace(mahh)) q = q.Where(x => x.MAHH == mahh);
        var data = await q.Select(x => x.ToReadDto()).ToListAsync();
        return Ok(data);
    }

    // GET: api/CtDonHang/{madon}/{mahh}
    [HttpGet("{madon}/{mahh}")]
    public async Task<ActionResult<CtDonHangReadDto>> GetById(string madon, string mahh)
    {
        var e = await _db.CtDonHangs.AsNoTracking()
            .FirstOrDefaultAsync(x => x.MADON == madon && x.MAHH == mahh);
        return e is null ? NotFound() : Ok(e.ToReadDto());
    }

    // POST: api/CtDonHang
    [HttpPost]
    public async Task<ActionResult<CtDonHangReadDto>> Create([FromBody] CtDonHangCreateDto dto)
    {
        // Validate cơ bản
        if (dto.SL < 0) return BadRequest("SL không được âm.");
        if (dto.DONGIA < 0) return BadRequest("DONGIA không được âm.");

        // Kiểm tra khóa chính kép trùng
        var exists = await _db.CtDonHangs.AnyAsync(x => x.MADON == dto.MADON && x.MAHH == dto.MAHH);
        if (exists) return Conflict($"Chi tiết đơn đã tồn tại: ({dto.MADON}, {dto.MAHH}).");

        // Kiểm tra FK
        var donExists = await _db.DonHangs.AnyAsync(d => d.MADON == dto.MADON);
        if (!donExists) return BadRequest($"MADON '{dto.MADON}' không tồn tại.");

        var hhExists = await _db.HangHoas.AnyAsync(h => h.MAHH == dto.MAHH);
        if (!hhExists) return BadRequest($"MAHH '{dto.MAHH}' không tồn tại.");

        var entity = dto.ToEntity();
        _db.CtDonHangs.Add(entity);
        await _db.SaveChangesAsync();

        var read = entity.ToReadDto();
        return CreatedAtAction(nameof(GetById), new { madon = entity.MADON, mahh = entity.MAHH }, read);
    }

    // PUT: api/CtDonHang/{madon}/{mahh}
    [HttpPut("{madon}/{mahh}")]
    public async Task<IActionResult> Update(string madon, string mahh, [FromBody] CtDonHangUpdateDto dto)
    {
        if (dto.SL < 0) return BadRequest("SL không được âm.");
        if (dto.DONGIA < 0) return BadRequest("DONGIA không được âm.");

        var entity = await _db.CtDonHangs.FirstOrDefaultAsync(x => x.MADON == madon && x.MAHH == mahh);
        if (entity is null) return NotFound();

        entity.Apply(dto);

        try
        {
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException e)
        {
            return Problem($"Lỗi cập nhật dữ liệu: {e.InnerException?.Message ?? e.Message}");
        }
    }

    // DELETE: api/CtDonHang/{madon}/{mahh}
    [HttpDelete("{madon}/{mahh}")]
    public async Task<IActionResult> Delete(string madon, string mahh)
    {
        var entity = await _db.CtDonHangs.FirstOrDefaultAsync(x => x.MADON == madon && x.MAHH == mahh);
        if (entity is null) return NotFound();

        _db.CtDonHangs.Remove(entity);
        try
        {
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException e)
        {
            return Conflict($"Không thể xóa do ràng buộc dữ liệu: {e.InnerException?.Message ?? e.Message}");
        }
    }
}
