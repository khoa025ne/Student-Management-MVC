using System;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity đại diện cho người dùng trong hệ thống
    /// </summary>
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; } = true;

        // Các trường mới
        public bool MustChangePassword { get; set; } = false;
        public string? GoogleId { get; set; }
        public DateTime? PasswordChangedAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? AvatarUrl { get; set; }

        // Navigation Properties
        public int RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;
    }
}
