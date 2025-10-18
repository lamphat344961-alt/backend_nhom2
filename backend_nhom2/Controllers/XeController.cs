using backend_nhom2.Data;
using backend_nhom2.Domain;
using backend_nhom2.Dtos.Xe;
using backend_nhom2.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_nhom2.Controllers;


[ApiController]
[Route("api/[controller]")]
public class XeController : ControllerBase
{
    private readonly AppDbContext _db;
    public XeController(AppDbContext db) => _db = db;


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Xe>>> GetAll() => await _db.Xes.AsNoTracking().ToListAsync();


    [HttpGet("{id}")]
    public async Task<ActionResult<Xe>> Get(string id)
    {
        var xe = await _db.Xes.FindAsync(id);
        return xe is null ? NotFound() : xe;
    }


    [HttpPost]
    public async Task<ActionResult<Xe>> Create(Xe xe)
    {
        _db.Xes.Add(xe);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = xe.BS_XE }, xe);
    }


    // PUT: api/Xe/{bsxe}
    [HttpPut("{bsxe}")]
    public async Task<IActionResult> Update(string bsxe, [FromBody] XeUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(bsxe)) return BadRequest("BS_XE không hợp lệ.");

        var xe = await _db.Xes.FirstOrDefaultAsync(x => x.BS_XE == bsxe);
        if (xe is null) return NotFound();

        // Validate đơn giản (nếu có quy ước)
        // if (dto.TT_XE is { Length: > 20 }) return BadRequest("TT_XE quá dài.");

        xe.Apply(dto);

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
        [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var xe = await _db.Xes.FindAsync(id);
        if (xe is null) return NotFound();


        // chặn xoá nếu có ràng buộc ct_diemgiao
        bool inUse = await _db.CtDiemGiaos.AnyAsync(x => x.BS_XE == id);
        if (inUse) return Conflict("Xe đang được sử dụng trong CT_DIEMGIAO.");


        _db.Xes.Remove(xe);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}