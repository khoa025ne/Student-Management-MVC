using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetAllAsync();
        Task<IEnumerable<Notification>> GetMyNotificationsAsync(int userId);
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task MarkAsReadAsync(int id);
        Task DeleteAsync(int id);
    }
}
