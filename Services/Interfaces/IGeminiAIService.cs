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

    public class LearningPathResult
    {
        public RecommendedCourse[] RecommendedCourses { get; set; } = Array.Empty<RecommendedCourse>();
        public string OverallStrategy { get; set; } = string.Empty;
        public string[] Warnings { get; set; } = Array.Empty<string>();
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class RecommendedCourse
    {
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
