using backend_nhom2.Data;
using backend_nhom2.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace backend_nhom2.Controllers;


[ApiController]
[Route("api/[controller]")]
public class DiemGiaoController : ControllerBase
{
    private readonly AppDbContext _db;
    public DiemGiaoController(AppDbContext db) => _db = db;


    [HttpGet]
    public async Task<IEnumerable<DiemGiao>> GetAll() => await _db.DiemGiaos.AsNoTracking().ToListAsync();


    [HttpPost]
    public async Task<ActionResult<DiemGiao>> Create(DiemGiao d)
    {
        _db.DiemGiaos.Add(d);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = d.IdDD }, d);
    }
}