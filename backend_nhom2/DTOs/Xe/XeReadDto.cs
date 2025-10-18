namespace backend_nhom2.DTOs.Xe
{
    public class XeReadDto
    {
        public required string BS_XE { get; set; }
        public string? TENXE { get; set; }
        public string? TT_XE { get; set; }
        public int? UserId { get; set; }
        public string? DriverFullName { get; set; } // Tên đầy đủ của tài xế
    }
}