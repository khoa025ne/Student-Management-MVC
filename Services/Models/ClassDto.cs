using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class ClassDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;
        public int? SemesterId { get; set; }
        public string? SemesterName { get; set; }
        public int? CourseId { get; set; }
        public string? CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxStudents { get; set; }
        public int MaxCapacity { get; set; } // Same as MaxStudents
        public int CurrentEnrollment { get; set; }
        public string? TeacherName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Schedule { get; set; }
        public string? Location { get; set; }
        public string? Room { get; set; }
        public int? TeacherId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Schedule fields - using DTO enums
        public DayOfWeekPair DayOfWeekPair { get; set; }
        public TimeSlot TimeSlot { get; set; }
        
        // Navigation properties - now using DTOs instead of Entities
        public CourseDto? Course { get; set; }
        public SemesterDto? Semester { get; set; }
    }

    public class ClassCreateDto
    {
        [Required(ErrorMessage = "Tên lớp là bắt buộc")]
        public string ClassName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã lớp là bắt buộc")]
        public string ClassCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Học kì là bắt buộc")]
        public int SemesterId { get; set; }

        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public int CourseId { get; set; }

        public int MaxStudents { get; set; } = 30;
        public int MaxCapacity { get; set; } = 30;
        public string? Room { get; set; }
        public string? Schedule { get; set; }
        public DayOfWeekPair DayOfWeekPair { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class ClassUpdateDto
    {
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Tên lớp là bắt buộc")]
        public string ClassName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã lớp là bắt buộc")]
        public string ClassCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Học kì là bắt buộc")]
        public int SemesterId { get; set; }

        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public int CourseId { get; set; }

        public int MaxStudents { get; set; } = 30;
        public int MaxCapacity { get; set; } = 30;
        public int CurrentEnrollment { get; set; }
        public string? Room { get; set; }
        public string? Schedule { get; set; }
        public DayOfWeekPair DayOfWeekPair { get; set; }
        public TimeSlot TimeSlot { get; set; }
        public int? TeacherId { get; set; }
        public string? TeacherName { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}