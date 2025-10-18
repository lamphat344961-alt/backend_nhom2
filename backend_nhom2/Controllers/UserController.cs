using backend_nhom2.Data;
using backend_nhom2.DTOs;
using backend_nhom2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_nhom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Owner")] // Chỉ Owner được truy cập controller này
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // Owner tạo tài khoản cho tài xế mới
        [HttpPost("create-driver")]
        public async Task<IActionResult> CreateDriver(RegisterRequestDto request)
        {
            if (request.Role.Trim().ToLower() != "driver")
            {
                return BadRequest("Chỉ có thể tạo tài khoản với vai trò 'Driver'.");
            }

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("Username đã tồn tại.");
            }

            var driverRole = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName == "Driver");
            if (driverRole == null)
            {
                // Trường hợp này hiếm khi xảy ra nếu đã seed data
                return StatusCode(500, "Không tìm thấy vai trò 'Driver' trong hệ thống.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                CCCD = request.CCCD,
                RoleId = driverRole.RoleId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Tạo tài khoản cho tài xế {user.FullName} thành công!" });
        }

        // Owner gán một tài xế vào một xe
        [HttpPut("assign-driver-to-vehicle")]
        public async Task<IActionResult> AssignDriverToVehicle([FromQuery] int driverId, [FromQuery] string vehiclePlate)
        {
            var driver = await _context.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == driverId);

            if (driver == null || driver.Role.RoleName != "Driver")
            {
                return NotFound("Không tìm thấy tài xế hợp lệ.");
            }

            var vehicle = await _context.Xes.FirstOrDefaultAsync(v => v.BS_XE == vehiclePlate);
            if (vehicle == null)
            {
                return NotFound("Không tìm thấy xe với biển số này.");
            }

            // Gán tài xế cho xe
            vehicle.UserId = driverId;
            await _context.SaveChangesAsync();

            return Ok($"Đã gán tài xế {driver.FullName} vào xe {vehicle.BS_XE}.");
        }
    }
}