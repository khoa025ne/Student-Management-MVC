using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    /// <summary>
    /// Interface cho Academic Analysis Repository
    /// </summary>
    public interface IAcademicAnalysisRepository
    {
        Task<IEnumerable<AcademicAnalysis>> GetAllAsync();
        Task<AcademicAnalysis?> GetByIdAsync(int id);
        Task<IEnumerable<AcademicAnalysis>> GetByStudentAsync(int studentId);
        Task<AcademicAnalysis?> GetLatestByStudentAsync(int studentId);
        Task<AcademicAnalysis> AddAsync(AcademicAnalysis analysis);
        Task<AcademicAnalysis> UpdateAsync(AcademicAnalysis analysis);
        Task DeleteAsync(int id);
    }
}
