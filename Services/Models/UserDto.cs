using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class UserDto
    {
        public int UserIdInt { get; set; } // int version
        public string UserId { get; set; } = string.Empty; // string version for backward compat
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneConfirmed { get; set; }
        public bool IsActive { get; set; }
        public bool MustChangePassword { get; set; }
        public string? GoogleId { get; set; }
        public DateTime? PasswordChangedAt { get; set; }
        public int RoleId { get; set; }
        public IList<RoleDto> Roles { get; set; } = new List<RoleDto>();
        
        // Navigation property - now using DTO instead of Entity
        public RoleDto? Role { get; set; }
    }

    public class UserCreateDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public List<int> RoleIds { get; set; } = new List<int>();
    }

    public class UserUpdateDto
    {
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public bool IsActive { get; set; } = true;
        public List<int> RoleIds { get; set; } = new List<int>();
    }

    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Color { get; set; }
        public bool IsActive { get; set; }
    }
}