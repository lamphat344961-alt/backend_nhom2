using backend_nhom2.Data;
using backend_nhom2.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace backend_nhom2.Controllers;


[ApiController]
[Route("api/[controller]")]
public class HangHoaController : ControllerBase
{
    private readonly AppDbContext _db;
    public HangHoaController(AppDbContext db) => _db = db;


    [HttpGet]
    public async Task<IEnumerable<HangHoa>> GetAll() => await _db.HangHoas.AsNoTracking().ToListAsync();


    [HttpPost]
    public async Task<ActionResult<HangHoa>> Create(HangHoa h)
    {
        _db.HangHoas.Add(h);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = h.MAHH }, h);
    }

    // Thêm các endpoint khác 
}