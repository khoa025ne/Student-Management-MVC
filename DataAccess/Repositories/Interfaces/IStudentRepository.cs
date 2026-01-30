using DataAccess.Entities;
using DataAccess.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    /// <summary>
    /// Interface cho Student Repository
    /// </summary>
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByCodeAsync(string studentCode);
        Task<IEnumerable<Student>> GetByMajorAsync(MajorType major);
        Task<IEnumerable<Student>> GetByTermAsync(int termNo);
        Task<IEnumerable<Student>> GetByClassCodeAsync(string classCode);
        Task<Student?> GetByUserIdAsync(int userId);
        Task<Student?> GetByUserIdWithEnrollmentsAsync(int userId);
        Task<int> GetUnreadNotificationCountAsync(int studentId);
        Task<Student> AddAsync(Student student);
        Task<Student> UpdateAsync(Student student);
        Task DeleteAsync(int id);
    }
}
