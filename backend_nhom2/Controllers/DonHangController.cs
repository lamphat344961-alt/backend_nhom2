using backend_nhom2.Data;
using backend_nhom2.Domain;
using backend_nhom2.DTOs.DonHang;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend_nhom2.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Owner,Driver")]
public class DonHangController : ControllerBase
{
    private readonly AppDbContext _db;
    public DonHangController(AppDbContext db) => _db = db;

    // GET api/DonHang
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DonHangReadDto>>> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] string? maloai = null,
        [FromQuery] string? q = null)
    {
        var query = _db.DonHangs.Include(d => d.DiemGiao).AsNoTracking();

        if (from.HasValue) query = query.Where(d => d.NGAYLAP >= from.Value);
        if (to.HasValue) query = query.Where(d => d.NGAYLAP < to.Value.AddDays(1));
        if (!string.IsNullOrWhiteSpace(maloai)) query = query.Where(d => d.MALOAI == maloai);
        if (!string.IsNullOrWhiteSpace(q)) query = query.Where(d => d.MADON.Contains(q));

        var data = await query
            .OrderByDescending(d => d.NGAYLAP)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DonHangReadDto(
                d.MADON, d.MALOAI, d.NGAYLAP, d.TONGTIEN,
                d.TRANGTHAI, d.D_DD, d.BS_XE,
                d.DiemGiao != null ? d.DiemGiao.TEN : null,
                d.DiemGiao != null ? d.DiemGiao.VITRI : null,
                d.DiemGiao != null ? d.DiemGiao.Lat : null,
                d.DiemGiao != null ? d.DiemGiao.Lng : null
            ))
            .ToListAsync();

        return Ok(data);
    }

    // GET api/DonHang/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<DonHangReadDto>> GetById(string id)
    {
        var don = await _db.DonHangs
            .Include(d => d.DiemGiao)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MADON == id);

        if (don == null)
            return NotFound($"Không tìm thấy đơn hàng với mã: {id}");

        var dto = new DonHangReadDto(
            don.MADON, don.MALOAI, don.NGAYLAP, don.TONGTIEN,
            don.TRANGTHAI, don.D_DD, don.BS_XE,
            don.DiemGiao?.TEN, don.DiemGiao?.VITRI, don.DiemGiao?.Lat, don.DiemGiao?.Lng
        );

        return Ok(dto);
    }

    // POST api/DonHang
    [HttpPost]
    public async Task<ActionResult<DonHangReadDto>> Create(DonHangCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.MADON))
            return BadRequest("MADON là bắt buộc.");

        if (await _db.DonHangs.AnyAsync(d => d.MADON == dto.MADON))
            return Conflict($"Mã đơn hàng '{dto.MADON}' đã tồn tại.");

        if (!string.IsNullOrWhiteSpace(dto.MALOAI) &&
            !await _db.LoaiHangs.AnyAsync(l => l.MALOAI == dto.MALOAI))
            return BadRequest($"MALOAI '{dto.MALOAI}' không tồn tại.");

        // 👇 Lấy UserId từ token (nếu có)
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int.TryParse(userIdStr, out var userId);

        // Nếu không nhập BS_XE thì auto gán
        string? bsXe = dto.BS_XE;
        if (string.IsNullOrEmpty(bsXe) && userId > 0)
        {
            bsXe = await _db.Xes
                .Where(x => x.UserId == userId)
                .Select(x => x.BS_XE)
                .FirstOrDefaultAsync();
        }

        var entity = new DonHang
        {
            MADON = dto.MADON,
            MALOAI = dto.MALOAI,
            NGAYLAP = dto.NGAYLAP,
            TONGTIEN = dto.TONGTIEN,
            BS_XE = bsXe,
            D_DD = dto.D_DD,
            TRANGTHAI = dto.TRANGTHAI ?? "CHO_GIAO",
            WindowStart = dto.WindowStart,
            WindowEnd = dto.WindowEnd,
            ServiceMinutes = dto.ServiceMinutes
        };

        _db.DonHangs.Add(entity);
        await _db.SaveChangesAsync();

        var read = new DonHangReadDto(
            entity.MADON, entity.MALOAI, entity.NGAYLAP, entity.TONGTIEN,
            entity.TRANGTHAI, entity.D_DD, entity.BS_XE,
            entity.DiemGiao?.TEN, entity.DiemGiao?.VITRI, entity.DiemGiao?.Lat, entity.DiemGiao?.Lng
        );

        return CreatedAtAction(nameof(GetById), new { id = entity.MADON }, read);
    }

    // PUT api/DonHang/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, DonHangUpdateDto dto)
    {
        var don = await _db.DonHangs.FindAsync(id);
        if (don == null) return NotFound($"Không tìm thấy đơn hàng: {id}");

        don.MALOAI = dto.MALOAI;
        don.NGAYLAP = dto.NGAYLAP;
        don.D_DD = dto.D_DD;
        don.WindowStart = dto.WindowStart;
        don.WindowEnd = dto.WindowEnd;
        don.ServiceMinutes = dto.ServiceMinutes;
        don.TRANGTHAI = dto.TRANGTHAI ?? don.TRANGTHAI;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // POST api/DonHang/{id}/complete
    [HttpPost("{id}/complete")]
    [Authorize(Roles = "Driver,Owner")]
    public async Task<IActionResult> CompleteOrder(string id)
    {
        var don = await _db.DonHangs.FirstOrDefaultAsync(d => d.MADON == id);
        if (don == null) return NotFound($"Không tìm thấy đơn hàng {id}");

        don.TRANGTHAI = "HOANTHANH";
        don.NGAYGIAO = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok($"Đơn hàng {id} đã hoàn thành.");
    }

    // DELETE api/DonHang/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var don = await _db.DonHangs
            .Include(d => d.CtDonHangs)
            .FirstOrDefaultAsync(d => d.MADON == id);

        if (don == null)
            return NotFound($"Không tìm thấy đơn hàng {id}");

        _db.DonHangs.Remove(don);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
