using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    /// <summary>
    /// Interface cho Course Repository
    /// </summary>
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(int id);
        Task<Course?> GetByCodeAsync(string courseCode);
        Task<Course> AddAsync(Course course);
        Task<Course> UpdateAsync(Course course);
        Task DeleteAsync(int id);
    }
}
