using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Models;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho AI Knowledge Base Service
    /// Quản lý knowledge base và tích hợp AI cho hệ thống
    /// </summary>
    public interface IAIKnowledgeBaseService
    {
        // CRUD Operations cho Knowledge Base
        Task<List<AIKnowledgeBaseModel>> GetAllAsync();
        Task<AIKnowledgeBaseModel?> GetByIdAsync(int id);
        Task<List<AIKnowledgeBaseModel>> GetByCategoryAsync(string category);
        Task<List<AIKnowledgeBaseModel>> SearchAsync(string query, string? category = null, int maxResults = 200);
        Task<AIKnowledgeBaseModel> CreateAsync(AIKnowledgeBaseModel model);
        Task<AIKnowledgeBaseModel> UpdateAsync(AIKnowledgeBaseModel model);
        Task<bool> DeleteAsync(int id);
        Task<bool> ToggleActiveAsync(int id);

        // AI Processing với Knowledge Base
        Task<AIProcessingResponse> ProcessWithKnowledgeBase(AIProcessingRequest request);
        
        // Academic Analysis (Flow 2)
        Task<AcademicAnalysisResponse> AnalyzeStudentPerformance(AcademicAnalysisRequest request);
        
        // Learning Path Recommendation (Flow 3)
        Task<LearningPathResponse> GenerateLearningPath(LearningPathRequest request);

        // Statistics
        Task<int> GetTotalCountAsync();
        Task<Dictionary<string, int>> GetCountByCategoryAsync();
        Task IncrementUsageCountAsync(int knowledgeId);

        // Seed Data
        Task SeedInitialKnowledgeBase();
    }
}
