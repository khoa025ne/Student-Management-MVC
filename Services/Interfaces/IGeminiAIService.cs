using Services.Models;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Gemini AI Service
    /// </summary>
    public interface IGeminiAIService
    {
        /// <summary>
        /// Phân tích học tập của sinh viên
        /// </summary>
        Task<AcademicAnalysisResult> AnalyzeStudentPerformanceAsync(int studentId);

        /// <summary>
        /// Gợi ý lộ trình học tập
        /// </summary>
        Task<LearningPathResult> GenerateLearningPathAsync(int studentId, int semesterId);
    }

    public class AcademicAnalysisResult
    {
        public string[] StrongSubjects { get; set; } = Array.Empty<string>();
        public string[] WeakSubjects { get; set; } = Array.Empty<string>();
        public string Recommendations { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
