using backend_nhom2.Data;
using backend_nhom2.Domain;
using backend_nhom2.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class DonHangController : ControllerBase
{
    private readonly AppDbContext _db;
    public DonHangController(AppDbContext db) => _db = db;


    [HttpPost]
    public async Task<ActionResult<DonHangReadDto>> Create(DonHangCreateDto dto)
    {
        using var tx = await _db.Database.BeginTransactionAsync();


        var don = new DonHang
        {
            MADON = dto.MADON,
            MALOAI = dto.MALOAI,
            NGAYLAP = dto.NGAYLAP,
            TONGTIEN = 0m
        };
        _db.DonHangs.Add(don);


        decimal total = 0m;
        foreach (var it in dto.Items)
        {
            var ct = new CtDonHang
            {
                MADON = dto.MADON,
                MAHH = it.MAHH,
                SL = it.SL,
                DONGIA = it.DONGIA
            };
            total += it.SL * it.DONGIA;
            _db.CtDonHangs.Add(ct);
        }
        don.TONGTIEN = total;


        await _db.SaveChangesAsync();
        await tx.CommitAsync();


        return await Get(dto.MADON);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<DonHangReadDto>> Get(string id)
    {
        var don = await _db.DonHangs
        .Include(x => x.CtDonHangs).ThenInclude(c => c.HangHoa)
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.MADON == id);


        if (don is null) return NotFound();


        var dto = new DonHangReadDto
        {
            MADON = don.MADON,
            NGAYLAP = don.NGAYLAP,
            TONGTIEN = don.TONGTIEN,
            MALOAI = don.MALOAI,
            Items = don.CtDonHangs.Select(c => new DonHangItemReadDto
            {
                MAHH = c.MAHH,
                TENHH = c.HangHoa?.TENHH,
                SL = c.SL,
                DONGIA = c.DONGIA
            }).ToList()
        };
        return dto;
    }

    // Thêm các endpoint khác 
}