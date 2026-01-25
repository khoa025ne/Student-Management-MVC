using DataAccess.Entities;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ILearningPathService
    {
        Task<LearningPathRecommendation?> GetLatestRecommendationAsync(int userId);
        Task<LearningPathRecommendation> GenerateRecommendationAsync(int userId);
    }
}
