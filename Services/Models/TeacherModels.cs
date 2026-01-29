namespace Services.Models
{
    /// <summary>
    /// DTO cho thông tin Teacher cơ bản
    /// </summary>
    public class TeacherDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string TeacherCode { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public DateTime? HireDate { get; set; }
    }

    /// <summary>
    /// DTO cho Teacher Dashboard
    /// </summary>
    public class TeacherDashboardDto
    {
        public TeacherDto Teacher { get; set; } = null!;
        public SemesterDto? CurrentSemester { get; set; }
        public int TotalClasses { get; set; }
        public int TotalStudents { get; set; }
        public List<TodayScheduleDto> TodaySchedule { get; set; } = new();
        public List<TeacherClassDto> MyClasses { get; set; } = new();
        public GradeDistributionDto GradeDistribution { get; set; } = new();
        public List<StudentAlertDto> StudentsNeedingAttention { get; set; } = new();
        public List<RecentScoreEntryDto> RecentScoreEntries { get; set; } = new();
        public int UnreadNotifications { get; set; }
        public List<ClassWithoutScoreDto> ClassesWithoutScores { get; set; } = new();
        
        // Shorthand properties for easy access in views
        public int GradeACount => GradeDistribution?.GradeACount ?? 0;
        public int GradeBCount => GradeDistribution?.GradeBCount ?? 0;
        public int GradeCCount => GradeDistribution?.GradeCCount ?? 0;
        public int GradeDCount => GradeDistribution?.GradeDCount ?? 0;
        public int GradeFCount => GradeDistribution?.GradeFCount ?? 0;
    }

    /// <summary>
    /// DTO cho lịch học hôm nay
    /// </summary>
    public class TodayScheduleDto
    {
        public int Id { get; set; }
        public string ClassCode { get; set; } = "";
        public string CourseName { get; set; } = "";
        public string Room { get; set; } = "";
        public string TimeSlot { get; set; } = "";
        public int StudentCount { get; set; }
        public int MaxCapacity { get; set; }
        public int MaxStudents => MaxCapacity; // Alias for view compatibility
    }

    /// <summary>
    /// DTO cho danh sách lớp của giáo viên
    /// </summary>
    public class TeacherClassDto
    {
        public int Id { get; set; }
        public string ClassCode { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string CourseName { get; set; } = "";
        public string CourseCode { get; set; } = "";
        public string Room { get; set; } = "";
        public string TimeSlot { get; set; } = "";
        public string Schedule { get; set; } = "";
        public int StudentCount { get; set; }
        public int MaxCapacity { get; set; }
        public int MaxStudents => MaxCapacity; // Alias for view compatibility
        public string SemesterName { get; set; } = "";
        public DateTime? SemesterStartDate { get; set; }
    }

    /// <summary>
    /// DTO cho chi tiết lớp học
    /// </summary>
    public class TeacherClassDetailDto
    {
        public int ClassId { get; set; }
        public string ClassCode { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string CourseName { get; set; } = "";
        public string CourseCode { get; set; } = "";
        public int Credits { get; set; }
        public string Room { get; set; } = "";
        public string TimeSlot { get; set; } = "";
        public string Schedule { get; set; } = "";
        public string SemesterName { get; set; } = "";
        public List<StudentEnrollmentDto> Students { get; set; } = new();
    }

    /// <summary>
    /// DTO cho sinh viên trong lớp
    /// </summary>
    public class StudentEnrollmentDto
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public double? AttendanceScore { get; set; }
        public double? Assignment1Score { get; set; }
        public double? Assignment2Score { get; set; }
        public double? MidtermScore { get; set; }
        public double? FinalExamScore { get; set; }
        public double? TotalScore { get; set; }
        public string? Grade { get; set; }
    }

    /// <summary>
    /// DTO cho phân phối điểm
    /// </summary>
    public class GradeDistributionDto
    {
        public int GradeACount { get; set; }
        public int GradeBCount { get; set; }
        public int GradeCCount { get; set; }
        public int GradeDCount { get; set; }
        public int GradeFCount { get; set; }
        public int TotalCount => GradeACount + GradeBCount + GradeCCount + GradeDCount + GradeFCount;
    }

    /// <summary>
    /// DTO cho sinh viên cần chú ý
    /// </summary>
    public class StudentAlertDto
    {
        public string StudentCode { get; set; } = "";
        public string StudentName { get; set; } = "";
        public string CourseName { get; set; } = "";
        public StudentAlertScoreDto? LatestScore { get; set; }
        public DateTime? ScoreDate { get; set; }
    }

    /// <summary>
    /// DTO cho điểm trong cảnh báo sinh viên
    /// </summary>
    public class StudentAlertScoreDto
    {
        public double? TotalScore { get; set; }
    }

    /// <summary>
    /// DTO cho điểm nhập gần đây
    /// </summary>
    public class RecentScoreEntryDto
    {
        public string StudentCode { get; set; } = "";
        public string StudentName { get; set; } = "";
        public string CourseName { get; set; } = "";
        public string ScoreType { get; set; } = "";
        public double? TotalScore { get; set; }
        public DateTime EntryDate { get; set; }
    }

    /// <summary>
    /// DTO cho lớp chưa nhập điểm
    /// </summary>
    public class ClassWithoutScoreDto
    {
        public int Id { get; set; }
        public string ClassCode { get; set; } = "";
        public string CourseName { get; set; } = "";
        public int StudentCount { get; set; }
    }

    /// <summary>
    /// DTO cho nhập điểm
    /// </summary>
    public class ScoreEntryDto
    {
        public int StudentId { get; set; }
        public double? AttendanceScore { get; set; }
        public double? Assignment1Score { get; set; }
        public double? Assignment2Score { get; set; }
        public double? MidtermScore { get; set; }
        public double? FinalExamScore { get; set; }
    }
}
