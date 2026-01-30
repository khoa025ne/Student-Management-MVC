using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetAllAsync();
        Task<IEnumerable<Notification>> GetByStudentIdAsync(int studentId);
        Task<Notification?> GetByIdAsync(int id);
        Task<Notification> AddAsync(Notification notification);
        Task<Notification> UpdateAsync(Notification notification);
        Task DeleteAsync(int id);
        Task MarkAsReadAsync(int id);
    }
}
