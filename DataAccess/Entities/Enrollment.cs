using System;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity đại diện cho việc đăng ký lớp học của sinh viên
    /// </summary>
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Active"; // Active, Cancelled, Failed

        // Navigation
        public virtual Student Student { get; set; } = null!;
        public virtual Class Class { get; set; } = null!;

        // Điểm số
        public double? MidtermScore { get; set; } // Điểm giữa kỳ
        public double? FinalScore { get; set; }   // Điểm cuối kỳ
        public double? TotalScore { get; set; }   // Tổng kết (VD: 40% Mid + 60% Final)

        public string? Grade { get; set; }  // A, B, C, D, F (Tính tự động)
        public bool IsPassed { get; set; } // True nếu qua môn

        // Đếm số lần học
        public int AttemptNumber { get; set; } = 1; // Lần học thứ mấy (1, 2, 3...)

        // Nhận xét từ giảng viên
        public string? Comment { get; set; } // Nhận xét, góp ý từ giảng viên
    }
}
