using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? CourseCode { get; set; }
        public string? Description { get; set; }
        public int Credits { get; set; }
        public string? Department { get; set; }
        public int? MajorId { get; set; }
        public string? MajorName { get; set; }
        public bool IsActive { get; set; }
        public double? AverageRating { get; set; }
        public int? TotalEnrollments { get; set; }
        public string? Prerequisites { get; set; }
    }

    public class CourseCreateDto
    {
        [Required(ErrorMessage = "Tên môn học là bắt buộc")]
        public string CourseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã môn học là bắt buộc")]
        public string CourseCode { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Số tín chỉ là bắt buộc")]
        [Range(1, 10, ErrorMessage = "Số tín chỉ phải từ 1 đến 10")]
        public int Credits { get; set; }

        public string? Department { get; set; }
        public int? MajorId { get; set; }
        public string? Prerequisites { get; set; }
    }

    public class CourseUpdateDto
    {
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Tên môn học là bắt buộc")]
        public string CourseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã môn học là bắt buộc")]
        public string CourseCode { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Số tín chỉ là bắt buộc")]
        [Range(1, 10, ErrorMessage = "Số tín chỉ phải từ 1 đến 10")]
        public int Credits { get; set; }

        public string? Department { get; set; }
        public int? MajorId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Prerequisites { get; set; }
    }

    public class SemesterDto
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int? TotalClasses { get; set; }
        public int? TotalEnrollments { get; set; }
    }

    public class SemesterCreateDto
    {
        [Required(ErrorMessage = "Tên học kì là bắt buộc")]
        public string SemesterName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime EndDate { get; set; }
    }

    public class SemesterUpdateDto
    {
        public int SemesterId { get; set; }

        [Required(ErrorMessage = "Tên học kì là bắt buộc")]
        public string SemesterName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; }
    }
}