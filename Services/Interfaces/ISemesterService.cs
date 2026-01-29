using DataAccess.Entities;
using Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Semester Service
    /// </summary>
    public interface ISemesterService
    {
        // Entity-based methods
        Task<IEnumerable<Semester>> GetAllAsync();
        Task<Semester?> GetByIdAsync(int id);
        Task<Semester?> GetActiveAsync();
        Task<Semester> CreateAsync(Semester semester);
        Task<Semester> UpdateAsync(Semester semester);
        Task DeleteAsync(int id);
        
        // DTO-based methods (for Controllers)
        Task<SemesterDto> CreateDtoAsync(SemesterCreateDto dto);
        Task<SemesterDto> UpdateDtoAsync(SemesterUpdateDto dto);
    }
}
