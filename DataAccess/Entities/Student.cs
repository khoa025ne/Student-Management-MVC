using System;
using System.Collections.Generic;
using DataAccess.Enums;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity đại diện cho sinh viên
    /// </summary>
    public class Student
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty; // STU202300145
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }

        public string ClassCode { get; set; } = string.Empty; // SE1801
        public double OverallGPA { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public MajorType Major { get; set; } = MajorType.Undefined; // Ngành học

        public string? AvatarUrl { get; set; } // "/uploads/sv123.jpg"

        // Kỳ học hiện tại
        public int? CurrentTermNo { get; set; }

        // Đăng nhập lần đầu
        public bool IsFirstLogin { get; set; } = true;

        // Navigation Properties
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
        public virtual ICollection<AcademicAnalysis> Analyses { get; set; } = new List<AcademicAnalysis>();
    }
}
