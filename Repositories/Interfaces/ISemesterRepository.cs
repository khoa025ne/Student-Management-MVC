using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    /// <summary>
    /// Interface cho Semester Repository
    /// </summary>
    public interface ISemesterRepository
    {
        Task<IEnumerable<Semester>> GetAllAsync();
        Task<Semester?> GetByIdAsync(int id);
        Task<Semester?> GetActiveAsync();
        Task<Semester> AddAsync(Semester semester);
        Task<Semester> UpdateAsync(Semester semester);
        Task DeleteAsync(int id);
    }
}
