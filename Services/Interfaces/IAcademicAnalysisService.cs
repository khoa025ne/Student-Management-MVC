using Services.Models;

namespace Services.Interfaces
{
    public interface IAcademicAnalysisService
    {
        Task<IEnumerable<AcademicAnalysisDto>> GetAllAnalysesAsync();
        Task<AcademicAnalysisDto?> GetAnalysisByIdAsync(int analysisId);
        Task<IEnumerable<AcademicAnalysisDto>> GetAnalysesByStudentIdAsync(string studentId);
        Task<AcademicAnalysisDto?> CreateAnalysisAsync(string studentId, string analysisType);
        Task<bool> DeleteAnalysisAsync(int analysisId);
        Task<AcademicAnalysisDto?> GenerateGpaAnalysisAsync(string studentId);
        Task<AcademicAnalysisDto?> GeneratePerformanceTrendAnalysisAsync(string studentId);
        Task<AcademicAnalysisDto?> GenerateLearningPathAnalysisAsync(string studentId);
        Task<IEnumerable<AcademicAnalysisDto>> GetStudentsAtRiskAsync();
        Task<bool> UpdateAnalysisStatusAsync(int analysisId, string status);
    }
}