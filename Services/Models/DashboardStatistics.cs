using System;
using System.Collections.Generic;

namespace Services.Models
{
    /// <summary>
    /// Model chứa tất cả thống kê cho Dashboard TailAdmin
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

        // TailAdmin - Growth Statistics
        public double StudentGrowthPercent { get; set; }
        public int NewStudentsThisMonth { get; set; }
        public double ClassGrowthPercent { get; set; }
        public int ActiveClassesThisSemester { get; set; }
        public double GPAChangePercent { get; set; }
        public int TotalCredits { get; set; }

        // TailAdmin - Academic Performance
        public double PassRate { get; set; }
        public double FailRate { get; set; }
        public int LowGPAStudents { get; set; }
        public int StudentsWithMultipleFails { get; set; }
        public int PendingEnrollments { get; set; }

        // TailAdmin - Grade Counts
        public int GradeACount { get; set; }
        public int GradeBCount { get; set; }
        public int GradeCCount { get; set; }
        public int GradeDCount { get; set; }
        public int GradeFCount { get; set; }
        public int TotalEnrollmentsWithGrade { get; set; }

        // TailAdmin - Monthly Trends
        public List<string> MonthlyLabels { get; set; } = new();
        public List<int> MonthlyEnrollments { get; set; } = new();
        public List<int> MonthlyNewStudents { get; set; } = new();

        // TailAdmin - Recent Activities
        public List<RecentActivity> RecentActivities { get; set; } = new();

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

    public class RecentActivity
    {
        public string Icon { get; set; } = "bi-circle-fill";
        public string IconColor { get; set; } = "text-info";
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = "info"; // info, success, warning, danger
        public DateTime Timestamp { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
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
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public string ClassCode { get; set; } = string.Empty;  // Added for TailAdmin
        public double GPA { get; set; }
        public double OverallGPA { get; set; }  // Added for TailAdmin (alias for GPA)
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
