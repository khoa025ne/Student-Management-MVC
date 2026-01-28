using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class StudentDto
    {
        public int StudentIdInt { get; set; } // int version
        public string StudentId { get; set; } = string.Empty; // string version for backward compat
        public int UserId { get; set; } // From Student entity
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? MajorName { get; set; }
        public int? MajorId { get; set; }
        public MajorType Major { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public string? ClassCode { get; set; }
        public string? Avatar { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Active";
        public string? StudentCode { get; set; }
        public string? ParentName { get; set; }
        public string? ParentPhone { get; set; }
        public string? ParentEmail { get; set; }
        public double? GPA { get; set; }
        public double OverallGPA { get; set; }
        public int? TotalCredits { get; set; }
        public int? CurrentTermNo { get; set; }
        public bool IsFirstLogin { get; set; }
        
        // Navigation property - now using DTO instead of Entity
        public UserDto? User { get; set; }
    }

    public class StudentCreateDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int? MajorId { get; set; }
        public int? ClassId { get; set; }
        public string? Avatar { get; set; }
        public string? StudentCode { get; set; }
        public string? ParentName { get; set; }
        public string? ParentPhone { get; set; }
        public string? ParentEmail { get; set; }
    }

    public class StudentUpdateDto
    {
        [Required]
        public string StudentId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int? MajorId { get; set; }
        public int? ClassId { get; set; }
        public string? Avatar { get; set; }
        public string? ParentName { get; set; }
        public string? ParentPhone { get; set; }
        public string? ParentEmail { get; set; }
        public string Status { get; set; } = "Active";
    }
}