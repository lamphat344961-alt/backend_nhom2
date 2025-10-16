using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_nhom2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Driver")] // Chỉ Driver được vào
    public class DriverController : ControllerBase
    {
        [HttpGet("today-deliveries")]
        public IActionResult GetTodayDeliveries()
        {
            var driverUsername = User.Identity?.Name ?? "Không xác định";
            return Ok($"Tài xế {driverUsername}, đây là các đơn hàng hôm nay của bạn.");
        }
    }
}
