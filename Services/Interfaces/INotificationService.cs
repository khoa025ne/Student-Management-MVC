using DataAccess.Entities;
using Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface INotificationService
    {
        // Entity-based methods
        Task<IEnumerable<Notification>> GetAllAsync();
        Task<IEnumerable<Notification>> GetMyNotificationsAsync(int userId);
        Task<Notification?> GetByIdAsync(int id);
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task MarkAsReadAsync(int id);
        Task DeleteAsync(int id);

        // Event-driven notification methods
        Task SendScoreUpdateNotificationAsync(int studentId, string courseName, double score, string grade);
        Task SendAchievementNotificationAsync(int studentId, string courseName, string grade);
        Task SendPerformanceAlertNotificationAsync(int studentId, string courseName, string grade, string reason);
        Task SendLearningPathNotificationAsync(int studentId, string[] recommendedCourses);
        
        // NotificationCenter methods (DTO-based)
        Task<NotificationCenterDto> GetNotificationCenterAsync(int userId, string userRole, string? type, bool? unreadOnly, int page);
        Task<int> MarkAsReadAndReturnIdAsync(int notificationId);
        Task<int> MarkAllAsReadAsync(int userId, string userRole);
        Task<IEnumerable<NotificationClassDto>> GetClassesForNotificationAsync();
        Task<int> CreateBulkNotificationsAsync(CreateNotificationDto model, int createdBy);
        Task SendGradeAlertAsync(int studentId, int classId, double score);
        Task<int> GetUnreadCountAsync(int userId, string userRole);
        
        // DTO-based methods (for Controllers)
        Task<NotificationDto> CreateNotificationDtoAsync(NotificationDto dto);
    }
}

