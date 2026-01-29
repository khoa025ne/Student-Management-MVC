using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    /// <summary>
    /// Interface cho Enrollment Repository
    /// </summary>
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(int id);
        Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId);
        Task<IEnumerable<Enrollment>> GetByClassAsync(int classId);
        Task<Enrollment> AddAsync(Enrollment enrollment);
        Task<Enrollment> UpdateAsync(Enrollment enrollment);
        Task DeleteAsync(int id);
        Task<bool> IsEnrolledAsync(int studentId, int classId);
        Task<Semester?> GetCurrentSemesterAsync();
    }
}
