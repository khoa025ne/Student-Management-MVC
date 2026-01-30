using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    public interface ILearningPathRepository
    {
        Task<IEnumerable<LearningPathRecommendation>> GetByStudentIdAsync(int studentId);
        Task<LearningPathRecommendation?> GetLatestByStudentAsync(int studentId);
        Task<LearningPathRecommendation> AddAsync(LearningPathRecommendation recommendation);
    }
}
