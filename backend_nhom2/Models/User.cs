namespace backend_nhom2.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? CCCD { get; set; }

        public DateTime CreatedAt { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}