using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface IScoreRepository
    {
        Task<IEnumerable<Score>> GetAllAsync();
        Task<IEnumerable<Score>> GetByStudentIdAsync(int studentId);
        Task<Score?> GetByIdAsync(int id);
        Task<Score?> GetByStudentAndCourseAsync(int studentId, int courseId);
        Task<Score> AddAsync(Score score);
        Task<Score> UpdateAsync(Score score);
        Task DeleteAsync(int id);
    }
}
