using DataAccess.Entities;
using Repositories.Interfaces;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IStudentService _studentService; // To find StudentId from UserId

        public NotificationService(INotificationRepository notificationRepo, IStudentService studentService)
        {
            _notificationRepo = notificationRepo;
            _studentService = studentService;
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _notificationRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Notification>> GetMyNotificationsAsync(int userId)
        {
            // Find Student for this user
            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                // If not student, maybe just return nothing or generic notifications if we support non-student notifs
                return Enumerable.Empty<Notification>();
            }
            return await _notificationRepo.GetByStudentIdAsync(student.StudentId);
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            return await _notificationRepo.AddAsync(notification);
        }

        public async Task MarkAsReadAsync(int id)
        {
            await _notificationRepo.MarkAsReadAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            await _notificationRepo.DeleteAsync(id);
        }
    }
}
