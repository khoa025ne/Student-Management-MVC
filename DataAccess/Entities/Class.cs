using System;
using System.Collections.Generic;
using DataAccess.Enums;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity đại diện cho lớp học
    /// </summary>
    public class Class
    {
        public int ClassId { get; set; }

        // Tên lớp: SE1801, AI1905...
        public string ClassName { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;

        // Sĩ số
        public int MaxCapacity { get; set; } = 30; // Mặc định 30
        public int CurrentEnrollment { get; set; } = 0;

        // Lịch học (Giản lược)
        public string? Room { get; set; } // P.304

        // Legacy: không dùng để check trùng lịch nữa, chỉ để hiển thị nếu cần
        public string? Schedule { get; set; } // Mon-Wed-Fri (Ca 1)

        // Lịch kiểu FPTU
        public DayOfWeekPair DayOfWeekPair { get; set; }   // 2-5, 3-6, 4-7
        public TimeSlot TimeSlot { get; set; }             // Slot1–4

        // Thuộc về Môn nào?
        public int CourseId { get; set; }
        public virtual Course Course { get; set; } = null!;

        // Thuộc về Học kỳ nào?
        public int SemesterId { get; set; }
        public virtual Semester Semester { get; set; } = null!;

        // Ai dạy? (Tạm thời để null nếu chưa làm Teacher)
        public int? TeacherId { get; set; }

        // Sĩ số tối đa (alias cho MaxCapacity)
        public int MaxStudents { get; set; } = 30; // Mặc định 30

        // Ngày tạo lớp học
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Danh sách sinh viên đăng ký vào lớp này
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
