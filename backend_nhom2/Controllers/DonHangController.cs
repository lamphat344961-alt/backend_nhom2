using backend_nhom2.Data;
using backend_nhom2.Domain;
using backend_nhom2.DTOs.DonHang;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_nhom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Owner")]
    public class DonHangController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DonHangController(AppDbContext db) => _db = db;

        // GET: api/DonHang - Lấy danh sách đơn hàng (không kèm chi tiết)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonHangReadDto>>> GetAll(
            [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null, [FromQuery] string? maloai = null, [FromQuery] string? q = null)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 20;

            var query = _db.DonHangs.AsNoTracking();

            if (from.HasValue) query = query.Where(d => d.NGAYLAP >= from.Value);
            if (to.HasValue) query = query.Where(d => d.NGAYLAP < to.Value.AddDays(1));
            if (!string.IsNullOrWhiteSpace(maloai)) query = query.Where(d => d.MALOAI == maloai);
            if (!string.IsNullOrWhiteSpace(q)) query = query.Where(d => d.MADON.Contains(q));

            var data = await query.OrderByDescending(d => d.NGAYLAP).Skip((pageIndex - 1) * pageSize).Take(pageSize)

                .Select(d => new DonHangReadDto(d.MADON, d.MALOAI, d.NGAYLAP, d.TONGTIEN))
                .ToListAsync();

            return Ok(data);
        }

        // GET: api/DonHang/{id} - Lấy thông tin một đơn hàng (không kèm chi tiết)
        [HttpGet("{id}")]
        public async Task<ActionResult<DonHangReadDto>> GetById(string id)
        {
            var don = await _db.DonHangs.AsNoTracking().FirstOrDefaultAsync(x => x.MADON == id);

            if (don is null) return NotFound($"Không tìm thấy đơn hàng với mã: {id}");

            // Trả về DTO đơn giản, không có Items
            var dto = new DonHangReadDto(don.MADON, don.MALOAI, don.NGAYLAP, don.TONGTIEN);
            return Ok(dto);
        }

        // POST: api/DonHang - Chỉ tạo thông tin chung của đơn hàng
        [HttpPost]
        public async Task<ActionResult<DonHangReadDto>> Create(DonHangCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MADON)) return BadRequest("MADON là bắt buộc.");
            if (await _db.DonHangs.AnyAsync(d => d.MADON == dto.MADON)) return Conflict($"Mã đơn hàng '{dto.MADON}' đã tồn tại.");
            if (!string.IsNullOrWhiteSpace(dto.MALOAI) && !await _db.LoaiHangs.AnyAsync(l => l.MALOAI == dto.MALOAI)) return BadRequest($"MALOAI '{dto.MALOAI}' không tồn tại.");

            var entity = new DonHang
            {
                MADON = dto.MADON,
                MALOAI = dto.MALOAI,
                NGAYLAP = dto.NGAYLAP,

                // Nó sẽ được cập nhật bởi CtDonHangController.
                TONGTIEN = 0
            };

            _db.DonHangs.Add(entity);
            await _db.SaveChangesAsync();

            var readDto = new DonHangReadDto(entity.MADON, entity.MALOAI, entity.NGAYLAP, entity.TONGTIEN);
            return CreatedAtAction(nameof(GetById), new { id = entity.MADON }, readDto);
        }

        // PUT: api/DonHang/{id} - Cập nhật thông tin chung của đơn hàng
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, DonHangUpdateDto dto)
        {
            var donHang = await _db.DonHangs.FindAsync(id);
            if (donHang is null) return NotFound($"Không tìm thấy đơn hàng với mã: {id}");

            donHang.MALOAI = dto.MALOAI;
            donHang.NGAYLAP = dto.NGAYLAP;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/DonHang/{id} - Xóa đơn hàng và tất cả chi tiết liên quan
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            // Dùng Include để EF Core biết cần xóa cả các chi tiết đơn hàng con
            var donHang = await _db.DonHangs.Include(d => d.CtDonHangs).FirstOrDefaultAsync(d => d.MADON == id);
            if (donHang is null) return NotFound($"Không tìm thấy đơn hàng với mã: {id}");

            // Khi xóa DonHang, các CtDonHang liên quan cũng sẽ bị xóa
            _db.DonHangs.Remove(donHang);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}