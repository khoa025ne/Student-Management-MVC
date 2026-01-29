using Services.Models;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Teacher Service - Xử lý nghiệp vụ giáo viên
    /// Controller chỉ được inject service này, không được truy cập DataAccess trực tiếp
    /// </summary>
    public interface ITeacherService
    {
        // ===== DASHBOARD =====
        Task<TeacherDashboardDto?> GetDashboardAsync(int teacherId);
        Task<TeacherDto?> GetByIdAsync(int teacherId);
        Task<TeacherDto?> GetByUserIdAsync(int userId);
        
        // ===== CLASS MANAGEMENT =====
        Task<IEnumerable<TeacherClassDto>> GetMyClassesAsync(int teacherId);
        Task<TeacherClassDetailDto?> GetClassDetailAsync(int classId);
        Task<TeacherClassDetailDto?> GetClassForGradeEntryAsync(int classId);
        
        // ===== GRADE MANAGEMENT =====
        Task<bool> SaveGradesAsync(int classId, List<ScoreEntryDto> scores);
        Task<IEnumerable<RecentScoreEntryDto>> GetRecentScoreEntriesAsync(int teacherId, int count = 5);
        Task<GradeDistributionDto> GetGradeDistributionAsync(int teacherId);
        
        // ===== STUDENTS =====
        Task<IEnumerable<StudentAlertDto>> GetStudentsNeedingAttentionAsync(int teacherId, int count = 5);
        
        // ===== NOTIFICATIONS =====
        Task<int> GetUnreadNotificationCountAsync(int teacherId);
    }
}
