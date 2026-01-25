using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IScoreService
    {
        Task<IEnumerable<Score>> GetAllAsync();
        Task<IEnumerable<Score>> GetByStudentIdAsync(int studentId);
        Task<Score?> GetByIdAsync(int id);
        Task<Score> AddOrUpdateScoreAsync(int studentId, int courseId, double scoreValue);
        Task DeleteAsync(int id);
    }
}
