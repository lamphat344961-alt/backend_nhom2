using backend_nhom2.Data;
using backend_nhom2.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace backend_nhom2.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CtDiemGiaoController : ControllerBase
{
    private readonly AppDbContext _db;
    public CtDiemGiaoController(AppDbContext db) => _db = db;


    // Upsert gán (điểm, xe) cho đơn
    [HttpPost]
    public async Task<IActionResult> Upsert(CtDiemGiao input)
    {
        var entity = await _db.CtDiemGiaos.FindAsync(input.IdDD, input.BS_XE, input.MADON);
        if (entity is null)
        {
            _db.CtDiemGiaos.Add(input);
        }
        else
        {
            entity.NGAYGIAO = input.NGAYGIAO;
            entity.TRANGTHAI = input.TRANGTHAI;
        }
        await _db.SaveChangesAsync();
        return Ok();
    }


    // Đánh dấu hoàn thành điểm giao
    [HttpPost("complete")]
    public async Task<IActionResult> Complete([FromQuery] string d_dd, [FromQuery] string bs_xe, [FromQuery] string madon)
    {
        var entity = await _db.CtDiemGiaos.FindAsync(d_dd, bs_xe, madon);
        if (entity is null) return NotFound();
        entity.TRANGTHAI = "HOANTHANH";
        entity.NGAYGIAO = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok();
    }


    [HttpGet("by-don/{madon}")]
    public async Task<IActionResult> ListByDon(string madon)
    {
        var list = await _db.CtDiemGiaos
        .Where(x => x.MADON == madon)
        .Include(x => x.DiemGiao)
        .Include(x => x.Xe)
        .AsNoTracking()
        .ToListAsync();
        return Ok(list);
    }
    // // Thêm các endpoint khác 
}