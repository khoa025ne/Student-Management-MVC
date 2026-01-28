using System;
using System.Collections.Generic;

namespace Services.Models
{
    /// <summary>
    /// Model chứa tất cả thống kê cho Dashboard
    /// </summary>
    public class DashboardStatistics
    {
        // Tổng quan
        public int TotalStudents { get; set; }
        public int TotalCourses { get; set; }
        public int TotalClasses { get; set; }
        public int TotalEnrollments { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalTeachers { get; set; }

        // Thống kê theo học kỳ hiện tại
        public int CurrentSemesterEnrollments { get; set; }
        public int CurrentSemesterClasses { get; set; }

        // Phân bố điểm số
        public ScoreDistribution ScoreStats { get; set; } = new();

        // Phân bố theo Major
        public Dictionary<string, int> StudentsByMajor { get; set; } = new();

        // Thống kê Enrollment theo status
        public Dictionary<string, int> EnrollmentsByStatus { get; set; } = new();

        // Top 10 học sinh xuất sắc
        public List<TopStudent> TopStudents { get; set; } = new();

        // Thống kê lớp học
        public List<ClassStatistic> ClassStatistics { get; set; } = new();

        // GPA trung bình
        public double AverageGPA { get; set; }

        // Tỷ lệ lấp đầy lớp
        public double ClassFillRate { get; set; }
    }

    public class ScoreDistribution
    {
        public int Excellent { get; set; }      // 9.0 - 10.0
        public int VeryGood { get; set; }       // 8.0 - 8.9
        public int Good { get; set; }           // 7.0 - 7.9
        public int Average { get; set; }        // 5.5 - 6.9
        public int BelowAverage { get; set; }   // 4.0 - 5.4
        public int Poor { get; set; }           // < 4.0
    }

    public class TopStudent
    {
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public double GPA { get; set; }
        public int Rank { get; set; }
    }

    public class ClassStatistic
    {
        public string ClassCode { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int MaxStudents { get; set; }
        public int CurrentEnrollment { get; set; }
        public double FillRate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
