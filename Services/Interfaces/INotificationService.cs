using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetAllAsync();
        Task<IEnumerable<Notification>> GetMyNotificationsAsync(int userId);
        Task<Notification> GetByIdAsync(int id);
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task MarkAsReadAsync(int id);
        Task DeleteAsync(int id);

        // Event-driven notification methods
        Task SendScoreUpdateNotificationAsync(int studentId, string courseName, double score, string grade);
        Task SendAchievementNotificationAsync(int studentId, string courseName, string grade);
        Task SendPerformanceAlertNotificationAsync(int studentId, string courseName, string grade, string reason);
        Task SendLearningPathNotificationAsync(int studentId, string[] recommendedCourses);
    }
}

