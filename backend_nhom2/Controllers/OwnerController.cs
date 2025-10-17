using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_nhom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Owner")] // Chỉ Owner được vào
    public class OwnerController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult GetOwnerDashboard()
        {
            var ownerUsername = User.Identity?.Name ?? "Không xác định";
            return Ok($"Chào mừng chủ xe {ownerUsername}. Đây là dữ liệu dashboard của bạn.");
        }

        [HttpPost("add-vehicle")]
        public IActionResult AddVehicle([FromBody] object vehicleData)
        {
            // Logic để thêm xe mới
            return Ok("Đã thêm xe mới thành công! (Chỉ Owner mới làm được)");
        }

        // Thêm các endpoint khác 
    }
}
