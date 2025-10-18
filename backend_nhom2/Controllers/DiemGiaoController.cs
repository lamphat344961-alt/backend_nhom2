using backend_nhom2.Data;
using backend_nhom2.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend_nhom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Owner")] // Chỉ Owner được quyền quản lý điểm giao
    public class DiemGiaoController : ControllerBase
    {
        private readonly AppDbContext _db;
        public DiemGiaoController(AppDbContext db) => _db = db;

        // GET: api/DiemGiao
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiemGiao>>> GetAll()
        {
            var diemGiaos = await _db.DiemGiaos.AsNoTracking().ToListAsync();
            return Ok(diemGiaos);
        }

        // GET: api/DiemGiao/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DiemGiao>> GetById(string id)
        {
            var diemGiao = await _db.DiemGiaos.FindAsync(id);
            if (diemGiao is null)
            {
                return NotFound($"Không tìm thấy điểm giao với ID: {id}");
            }
            return Ok(diemGiao);
        }

        // POST: api/DiemGiao
        [HttpPost]
        public async Task<ActionResult<DiemGiao>> Create(DiemGiao diemGiao)
        {
            if (await _db.DiemGiaos.AnyAsync(d => d.IdDD == diemGiao.IdDD))
            {
                return Conflict($"Điểm giao với ID '{diemGiao.IdDD}' đã tồn tại.");
            }

            _db.DiemGiaos.Add(diemGiao);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = diemGiao.IdDD }, diemGiao);
        }

        // PUT: api/DiemGiao/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, DiemGiao diemGiao)
        {
            if (id != diemGiao.IdDD)
            {
                return BadRequest("ID trong URL và trong body không khớp.");
            }

            var entity = await _db.DiemGiaos.FindAsync(id);
            if (entity is null)
            {
                return NotFound($"Không tìm thấy điểm giao để cập nhật với ID: {id}");
            }

            entity.TEN = diemGiao.TEN;
            entity.VITRI = diemGiao.VITRI;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/DiemGiao/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var diemGiao = await _db.DiemGiaos.FindAsync(id);
            if (diemGiao is null)
            {
                return NotFound($"Không tìm thấy điểm giao để xóa với ID: {id}");
            }

            _db.DiemGiaos.Remove(diemGiao);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}