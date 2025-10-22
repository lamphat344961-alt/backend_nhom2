using System.Security.Claims;
using backend_nhom2.Data;
using backend_nhom2.DTOs.DonHang;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_nhom2.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Driver")]
public class DriverController : ControllerBase
{
    private readonly AppDbContext _db;
    public DriverController(AppDbContext db) => _db = db;

    // GET: /api/Driver/my-deliveries
    [HttpGet("my-deliveries")]
    public async Task<ActionResult<IEnumerable<DonHangReadDto>>> MyDeliveries()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized("Không xác định được tài xế đang đăng nhập.");

        // Tìm xe gán cho tài xế hiện tại (AppDbContext có unique index Xe.UserId)
        var myPlate = await _db.Xes
            .Where(x => x.UserId == userId)
            .Select(x => x.BS_XE)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(myPlate))
            return Ok(Enumerable.Empty<DonHangReadDto>()); // tài xế chưa được gán xe → chưa có đơn

        // Lọc đơn theo BS_XE của xe tài xế
        var query = _db.DonHangs
            .Include(d => d.DiemGiao)
            .AsNoTracking()
            .Where(d => d.BS_XE == myPlate);

        // Nếu muốn ẩn đơn đã hoàn thành thì thêm điều kiện:
        // query = query.Where(d => d.TRANGTHAI != "HOANTHANH");

        var data = await query
            .OrderBy(d => d.NGAYLAP)
            .Select(d => new DonHangReadDto(
                d.MADON,
                d.MALOAI,
                d.NGAYLAP,
                d.TONGTIEN,
                d.TRANGTHAI,
                d.D_DD,
                d.BS_XE,                                  // 👈 map BS_XE
                d.DiemGiao != null ? d.DiemGiao.TEN : null,
                d.DiemGiao != null ? d.DiemGiao.VITRI : null,
                d.DiemGiao != null ? d.DiemGiao.Lat : null,
                d.DiemGiao != null ? d.DiemGiao.Lng : null
            ))
            .ToListAsync();

        return Ok(data);
    }

    // POST: /api/Driver/complete/{maDon}
    [HttpPost("complete/{maDon}")]
    public async Task<IActionResult> Complete(string maDon)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        // Xác định xe của tài xế
        var myPlate = await _db.Xes
            .Where(x => x.UserId == userId)
            .Select(x => x.BS_XE)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(myPlate))
            return BadRequest("Bạn chưa được gán xe, không thể hoàn thành đơn.");

        // Chỉ cho phép hoàn thành đơn thuộc xe của chính mình
        var don = await _db.DonHangs.FirstOrDefaultAsync(d => d.MADON == maDon && d.BS_XE == myPlate);
        if (don == null) return NotFound($"Không tìm thấy đơn {maDon} của tài xế hiện tại.");

        don.TRANGTHAI = "HOANTHANH";
        don.NGAYGIAO = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok($"Đơn {maDon} đã hoàn thành.");
    }

    // Helper: lấy userId từ JWT
    private int? GetCurrentUserId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idStr, out var id)) return id;
        return null;
        // Nếu dự án của bạn lưu claim kiểu khác (vd "sub"), có thể bổ sung:
        // var sub = User.FindFirstValue("sub"); ...
    }
}
