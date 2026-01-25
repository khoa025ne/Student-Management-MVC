using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Semester Service
    /// </summary>
    public interface ISemesterService
    {
        Task<IEnumerable<Semester>> GetAllAsync();
        Task<Semester?> GetByIdAsync(int id);
        Task<Semester?> GetActiveAsync();
        Task<Semester> CreateAsync(Semester semester);
        Task<Semester> UpdateAsync(Semester semester);
        Task DeleteAsync(int id);
    }
}
