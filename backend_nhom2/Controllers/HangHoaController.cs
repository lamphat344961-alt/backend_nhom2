// path: backend_nhom2/Controllers/HangHoaController.cs
using backend_nhom2.Data;
using backend_nhom2.Domain;
using backend_nhom2.Dtos.HangHoa;
using backend_nhom2.DTOs.HangHoa;
using backend_nhom2.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_nhom2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HangHoaController : ControllerBase
{
    private readonly AppDbContext _db;
    public HangHoaController(AppDbContext db) => _db = db;

    // GET: api/HangHoa
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HangHoaReadDto>>> GetAll()
        => Ok(await _db.HangHoas.AsNoTracking().Select(h => h.ToReadDto()).ToListAsync());

    // GET: api/HangHoa/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<HangHoaReadDto>> GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return BadRequest("MAHH không hợp lệ.");
        var hh = await _db.HangHoas.AsNoTracking().FirstOrDefaultAsync(x => x.MAHH == id);
        return hh is null ? NotFound() : Ok(hh.ToReadDto());
    }

    // POST: api/HangHoa
    [HttpPost]
    public async Task<ActionResult<HangHoaReadDto>> Create([FromBody] HangHoaCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.MAHH)) return BadRequest("MAHH là bắt buộc.");
        var exists = await _db.HangHoas.AnyAsync(x => x.MAHH == dto.MAHH);
        if (exists) return Conflict($"MAHH '{dto.MAHH}' đã tồn tại.");

        var entity = dto.ToEntity();
        _db.HangHoas.Add(entity);
        await _db.SaveChangesAsync();

        var read = entity.ToReadDto();
        return CreatedAtAction(nameof(GetById), new { id = entity.MAHH }, read);
    }

    // PUT: api/HangHoa/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] HangHoaUpdateDto dto)
    {
        var entity = await _db.HangHoas.FirstOrDefaultAsync(x => x.MAHH == id);
        if (entity is null) return NotFound();

        entity.Apply(dto);

        try
        {
            await _db.SaveChangesAsync();
            return NoContent();
        }
        catch (DbUpdateException e)
        {
            return Problem($"Lỗi cập nhật dữ liệu: {e.Message}");
        }
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
        catch (DbUpdateException e)
        {
            return Conflict($"Không thể xóa do ràng buộc dữ liệu: {e.Message}");
        }
    }
}
