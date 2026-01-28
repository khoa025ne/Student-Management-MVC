using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class ClassDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int? SemesterId { get; set; }
        public string? SemesterName { get; set; }
        public int? CourseId { get; set; }
        public string? CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int MaxStudents { get; set; }
        public int CurrentEnrollment { get; set; }
        public string? TeacherName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Schedule { get; set; }
        public string? Location { get; set; }
    }

    public class ClassCreateDto
    {
        [Required(ErrorMessage = "Tên lớp là bắt buộc")]
        public string ClassName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Học kì là bắt buộc")]
        public int SemesterId { get; set; }

        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Số lượng sinh viên tối đa là bắt buộc")]
        public int MaxStudents { get; set; }

        public string? TeacherName { get; set; }
        public string? Description { get; set; }
        public string? Schedule { get; set; }
        public string? Location { get; set; }
    }

    public class ClassUpdateDto
    {
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Tên lớp là bắt buộc")]
        public string ClassName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Học kì là bắt buộc")]
        public int SemesterId { get; set; }

        [Required(ErrorMessage = "Môn học là bắt buộc")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Số lượng sinh viên tối đa là bắt buộc")]
        public int MaxStudents { get; set; }

        public string? TeacherName { get; set; }
        public string? Description { get; set; }
        public string? Schedule { get; set; }
        public string? Location { get; set; }
        public bool IsActive { get; set; } = true;
    }
}