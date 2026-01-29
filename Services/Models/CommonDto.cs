namespace Services.Models
{
    public class AcademicAnalysisDto
    {
        public int AnalysisId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string? StudentName { get; set; }
        public double OverallGPA { get; set; }
        public List<string> StrongSubjects { get; set; } = new();
        public List<string> WeakSubjects { get; set; } = new();
        public string? Recommendations { get; set; }
        public DateTime AnalysisDate { get; set; }
        public string AiModelUsed { get; set; } = string.Empty;
        
        // Legacy properties for compatibility
        public string AnalysisType { get; set; } = "GPA";
        public string Content { get; set; } = string.Empty;
        public DateTime GeneratedDate => AnalysisDate;
        public bool IsActive { get; set; } = true;
        public double? CurrentGPA => OverallGPA;
        public string? RiskLevel => OverallGPA < 5.0 ? "High" : OverallGPA < 7.0 ? "Medium" : "Low";
        public string? Status { get; set; } = "Active";
    }

    // NotificationDto moved to NotificationModels.cs

    public class DashboardStatsDto
    {
        public int TotalStudents { get; set; }
        public int ActiveStudents { get; set; }
        public int TotalClasses { get; set; }
        public int ActiveClasses { get; set; }
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public double AverageGPA { get; set; }
        public int StudentsAtRisk { get; set; }
        public int RecentNotifications { get; set; }
        public List<GpaDistributionDto> GpaDistribution { get; set; } = new();
        public List<EnrollmentTrendDto> EnrollmentTrends { get; set; } = new();
    }

    public class GpaDistributionDto
    {
        public string Range { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class EnrollmentTrendDto
    {
        public string SemesterName { get; set; } = string.Empty;
        public int EnrollmentCount { get; set; }
        public DateTime Date { get; set; }
    }
}