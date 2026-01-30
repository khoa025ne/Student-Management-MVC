using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    /// <summary>
    /// Interface cho Class Repository
    /// </summary>
    public interface IClassRepository
    {
        Task<IEnumerable<Class>> GetAllAsync();
        Task<Class?> GetByIdAsync(int id);
        Task<IEnumerable<Class>> GetBySemesterAsync(int semesterId);
        Task<IEnumerable<Class>> GetByCourseAsync(int courseId);
        Task<Class> AddAsync(Class classEntity);
        Task<Class> UpdateAsync(Class classEntity);
        Task DeleteAsync(int id);
    }
}
