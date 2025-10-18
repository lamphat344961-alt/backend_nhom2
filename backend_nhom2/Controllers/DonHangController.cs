// path: backend_nhom2/Controllers/DonHangController.cs
using backend_nhom2.Data;
using backend_nhom2.Domain;
using backend_nhom2.Dtos.DonHang;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_nhom2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonHangController : ControllerBase
{
    private readonly AppDbContext _db;
    public DonHangController(AppDbContext db) => _db = db;

    // GET: api/DonHang
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DonHangReadDto>>> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? maloai = null,
        [FromQuery] string? q = null
    )
    {
        if (pageIndex < 1) pageIndex = 1;
        if (pageSize < 1 || pageSize > 200) pageSize = 20;

        var query = _db.DonHangs.AsNoTracking().AsQueryable();

        if (from.HasValue) query = query.Where(d => d.NGAYLAP >= from.Value);
        if (to.HasValue) query = query.Where(d => d.NGAYLAP < to.Value.AddDays(1));
        if (!string.IsNullOrWhiteSpace(maloai)) query = query.Where(d => d.MALOAI == maloai);
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(d => d.MADON.Contains(q));

        var data = await query
            .OrderByDescending(d => d.NGAYLAP)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DonHangReadDto(
                d.MADON,
                d.MALOAI,
                d.NGAYLAP,
                d.TONGTIEN
            ))
            .ToListAsync();

        return Ok(data);
    }

    // POST: api/DonHang
    [HttpPost]
    public async Task<ActionResult<DonHangReadDto>> Create([FromBody] DonHangCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.MADON))
            return BadRequest("MADON là bắt buộc.");

        // Kiểm tra trùng khóa
        var exists = await _db.DonHangs.AnyAsync(d => d.MADON == dto.MADON);
        if (exists)
            return Conflict($"Mã đơn hàng '{dto.MADON}' đã tồn tại.");

        // Kiểm tra loại hàng (FK)
        if (!string.IsNullOrWhiteSpace(dto.MALOAI))
        {
            var loaiExists = await _db.LoaiHangs.AnyAsync(l => l.MALOAI == dto.MALOAI);
            if (!loaiExists)
                return BadRequest($"MALOAI '{dto.MALOAI}' không tồn tại.");
        }

        var entity = new DonHang
        {
            MADON = dto.MADON,
            MALOAI = dto.MALOAI,
            NGAYLAP = dto.NGAYLAP,
            TONGTIEN = dto.TONGTIEN
        };

        _db.DonHangs.Add(entity);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            return Problem($"Lỗi lưu dữ liệu: {e.InnerException?.Message ?? e.Message}");
        }

        var read = new DonHangReadDto(entity.MADON, entity.MALOAI, entity.NGAYLAP, entity.TONGTIEN);
        return CreatedAtAction(nameof(GetAll), new { id = entity.MADON }, read);
    }
}
