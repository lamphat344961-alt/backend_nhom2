using backend_nhom2.Data;
using backend_nhom2.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace backend_nhom2.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CtDonHangController : ControllerBase
{
    private readonly AppDbContext _db;
    public CtDonHangController(AppDbContext db) => _db = db;


    [HttpPost]
    public async Task<IActionResult> AddItem([FromBody] CtDonHang input)
    {
        var exists = await _db.CtDonHangs.FindAsync(input.MAHH, input.MADON);
        if (exists != null) return Conflict("Mặt hàng đã tồn tại trong đơn.");


        _db.CtDonHangs.Add(input);
        await _db.SaveChangesAsync();


        // cập nhật tổng tiền đơn
        var don = await _db.DonHangs.FindAsync(input.MADON);
        if (don != null)
        {
            don.TONGTIEN += input.SL * input.DONGIA;
            await _db.SaveChangesAsync();
        }
        return Ok();
    }
}