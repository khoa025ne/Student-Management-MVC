using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    public class EnrollmentDto
    {
        public int EnrollmentId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string? StudentName { get; set; }
        public int ClassId { get; set; }
        public string? ClassName { get; set; }
        public string? CourseName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = "Active";
        public string? Comment { get; set; }
        public double? FinalGrade { get; set; }
        public string? LetterGrade { get; set; }
        public double? AttendanceRate { get; set; }
    }

    public class EnrollmentCreateDto
    {
        [Required(ErrorMessage = "Sinh viên là bắt buộc")]
        public string StudentId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lớp học là bắt buộc")]
        public int ClassId { get; set; }

        public string? Comment { get; set; }
    }

    public class EnrollmentUpdateDto
    {
        public int EnrollmentId { get; set; }
        public string Status { get; set; } = "Active";
        public string? Comment { get; set; }
        public double? FinalGrade { get; set; }
        public string? LetterGrade { get; set; }
        public double? AttendanceRate { get; set; }
    }

    public class ScoreDto
    {
        public int ScoreId { get; set; }
        public int EnrollmentId { get; set; }
        public string? StudentName { get; set; }
        public string? CourseName { get; set; }
        public string ScoreType { get; set; } = string.Empty;
        public double ScoreValue { get; set; }
        public double Weight { get; set; }
        public DateTime ExamDate { get; set; }
        public string? Comment { get; set; }
    }

    public class ScoreCreateDto
    {
        [Required(ErrorMessage = "Đăng ký học là bắt buộc")]
        public int EnrollmentId { get; set; }

        [Required(ErrorMessage = "Loại điểm là bắt buộc")]
        public string ScoreType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Điểm số là bắt buộc")]
        [Range(0, 10, ErrorMessage = "Điểm số phải từ 0 đến 10")]
        public double ScoreValue { get; set; }

        [Required(ErrorMessage = "Trọng số là bắt buộc")]
        [Range(0, 1, ErrorMessage = "Trọng số phải từ 0 đến 1")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Ngày thi là bắt buộc")]
        public DateTime ExamDate { get; set; }

        public string? Comment { get; set; }
    }
}