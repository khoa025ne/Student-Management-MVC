using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Class Service
    /// </summary>
    public interface IClassService
    {
        Task<IEnumerable<Class>> GetAllAsync();
        Task<Class?> GetByIdAsync(int id);
        Task<IEnumerable<Class>> GetBySemesterAsync(int semesterId);
        Task<IEnumerable<Class>> GetByCourseAsync(int courseId);
        Task<Class> CreateAsync(Class classEntity);
        Task<Class> UpdateAsync(Class classEntity);
        Task DeleteAsync(int id);
    }
}
