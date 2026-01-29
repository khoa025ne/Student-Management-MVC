namespace Services.Models
{
    /// <summary>
    /// DTO cho Student Dashboard
    /// Dùng cho Controller thay vì truy cập Entity trực tiếp
    /// </summary>
    public class StudentDashboardDto
    {
        public StudentInfoDto Student { get; set; } = null!;
        public SemesterDto? CurrentSemester { get; set; }
        public double OverallGPA { get; set; }
        public int TotalCredits { get; set; }
        public int TotalCreditsThisSemester { get; set; }
        public int EnrolledCoursesCount { get; set; }
        public int CompletedCoursesCount { get; set; }
        public int GradeACount { get; set; }
        public int GradeBCount { get; set; }
        public int GradeCCount { get; set; }
        public int GradeDCount { get; set; }
        public int GradeFCount { get; set; }
        public List<RecentScoreDto> RecentScores { get; set; } = new();
        public List<UpcomingClassDto> UpcomingClasses { get; set; } = new();
        public int UnreadNotifications { get; set; }
        public bool NeedsAcademicWarning { get; set; }
        public double PassRate { get; set; }
    }

    /// <summary>
    /// DTO cho thông tin Student cơ bản
    /// </summary>
    public class StudentInfoDto
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string? Major { get; set; }
        public string? ClassCode { get; set; }
        public string? Status { get; set; }
        public double OverallGPA { get; set; }
    }

    /// <summary>
    /// DTO cho điểm gần đây
    /// </summary>
    public class RecentScoreDto
    {
        public string CourseName { get; set; } = "";
        public string CourseCode { get; set; } = "";
        public string? Grade { get; set; }
        public double? TotalScore { get; set; }
        public int Credits { get; set; }
    }

    /// <summary>
    /// DTO cho lớp học sắp tới
    /// </summary>
    public class UpcomingClassDto
    {
        public string ClassName { get; set; } = "";
        public string CourseName { get; set; } = "";
        public string Room { get; set; } = "";
        public string TimeSlot { get; set; } = "";
    }
}
