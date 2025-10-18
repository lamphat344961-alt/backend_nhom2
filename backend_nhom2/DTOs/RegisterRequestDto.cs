namespace backend_nhom2.DTOs
{
    public class RegisterRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? CCCD { get; set; }
        public string Role { get; set; } = string.Empty; // Client sẽ gửi lên "Owner" hoặc "Driver"
    }
}