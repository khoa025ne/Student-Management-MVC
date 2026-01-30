using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Services.Interfaces;
using Services.Models;
using Newtonsoft.Json;

namespace Services.Implementations
{
    /// <summary>
    /// Service xử lý Academic Analysis - Logic nghiệp vụ phân tích học tập
    /// </summary>
    public class AcademicAnalysisService : IAcademicAnalysisService
    {
        private readonly IAcademicAnalysisRepository _analysisRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IGeminiAIService _geminiAIService;

        public AcademicAnalysisService(
            IAcademicAnalysisRepository analysisRepository,
            IStudentRepository studentRepository,
            IEnrollmentRepository enrollmentRepository,
            IGeminiAIService geminiAIService)
        {
            _analysisRepository = analysisRepository;
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
            _geminiAIService = geminiAIService;
        }

        public async Task<IEnumerable<AcademicAnalysisDto>> GetAllAnalysesAsync()
        {
            var analyses = await _analysisRepository.GetAllAsync();
            return analyses.Select(MapToDto);
        }

        public async Task<AcademicAnalysisDto?> GetAnalysisByIdAsync(int analysisId)
        {
            var analysis = await _analysisRepository.GetByIdAsync(analysisId);
            return analysis != null ? MapToDto(analysis) : null;
        }

        public async Task<IEnumerable<AcademicAnalysisDto>> GetAnalysesByStudentIdAsync(string studentId)
        {
            if (!int.TryParse(studentId, out int id))
                return Enumerable.Empty<AcademicAnalysisDto>();
            
            var analyses = await _analysisRepository.GetByStudentAsync(id);
            return analyses.Select(MapToDto);
        }

        public async Task<AcademicAnalysisDto?> CreateAnalysisAsync(string studentId, string analysisType)
        {
            if (!int.TryParse(studentId, out int id))
                return null;

            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return null;

            AcademicAnalysisDto? analysisResult = null;

            switch (analysisType.ToLower())
            {
                case "gpa":
                    analysisResult = await GenerateGpaAnalysisAsync(studentId);
                    break;
                case "performance":
                    analysisResult = await GeneratePerformanceTrendAnalysisAsync(studentId);
                    break;
                case "learningpath":
                    analysisResult = await GenerateLearningPathAnalysisAsync(studentId);
                    break;
                default:
                    throw new ArgumentException($"Unsupported analysis type: {analysisType}");
            }

            return analysisResult;
        }

        public async Task<bool> DeleteAnalysisAsync(int analysisId)
        {
            var analysis = await _analysisRepository.GetByIdAsync(analysisId);
            if (analysis == null)
                return false;

            await _analysisRepository.DeleteAsync(analysisId);
            return true;
        }

        public async Task<AcademicAnalysisDto?> GenerateGpaAnalysisAsync(string studentId)
        {
            try
            {
                if (!int.TryParse(studentId, out int id))
                    return null;

                var student = await _studentRepository.GetByIdAsync(id);
                if (student == null)
                    return null;

                // Lấy enrollments của sinh viên
                var enrollments = await _enrollmentRepository.GetByStudentAsync(id);
                
                // Tạo AI analysis
                var aiResult = await _geminiAIService.AnalyzeStudentPerformanceAsync(id);
                
                var analysis = new AcademicAnalysis
                {
                    StudentId = id,
                    AnalysisDate = DateTime.Now,
                    OverallGPA = student.OverallGPA,
                    StrongSubjectsJson = aiResult.Success ? JsonConvert.SerializeObject(aiResult.StrongSubjects) : "[]",
                    WeakSubjectsJson = aiResult.Success ? JsonConvert.SerializeObject(aiResult.WeakSubjects) : "[]",
                    Recommendations = aiResult.Success ? aiResult.Recommendations : "Không thể tạo phân tích",
                    AiModelUsed = "Gemini-1.5-Pro"
                };

                var createdAnalysis = await _analysisRepository.AddAsync(analysis);
                return MapToDto(createdAnalysis);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating GPA analysis: {ex.Message}");
                return null;
            }
        }

        public async Task<AcademicAnalysisDto?> GeneratePerformanceTrendAnalysisAsync(string studentId)
        {
            // Same logic as GPA analysis for now
            return await GenerateGpaAnalysisAsync(studentId);
        }

        public async Task<AcademicAnalysisDto?> GenerateLearningPathAnalysisAsync(string studentId)
        {
            // Same logic as GPA analysis for now
            return await GenerateGpaAnalysisAsync(studentId);
        }

        public async Task<IEnumerable<AcademicAnalysisDto>> GetStudentsAtRiskAsync()
        {
            var allAnalyses = await _analysisRepository.GetAllAsync();
            // Students with GPA < 5.0 are at risk
            var atRiskAnalyses = allAnalyses.Where(a => a.OverallGPA < 5.0);
            return atRiskAnalyses.Select(MapToDto);
        }

        public async Task<bool> UpdateAnalysisStatusAsync(int analysisId, string status)
        {
            var analysis = await _analysisRepository.GetByIdAsync(analysisId);
            if (analysis == null)
                return false;

            // AcademicAnalysis entity doesn't have status field, just return true
            await _analysisRepository.UpdateAsync(analysis);
            return true;
        }

        private AcademicAnalysisDto MapToDto(AcademicAnalysis analysis)
        {
            return new AcademicAnalysisDto
            {
                AnalysisId = analysis.AnalysisId,
                StudentId = analysis.StudentId.ToString(),
                StudentName = analysis.Student?.FullName,
                OverallGPA = analysis.OverallGPA,
                StrongSubjects = !string.IsNullOrEmpty(analysis.StrongSubjectsJson) 
                    ? JsonConvert.DeserializeObject<List<string>>(analysis.StrongSubjectsJson) ?? new List<string>()
                    : new List<string>(),
                WeakSubjects = !string.IsNullOrEmpty(analysis.WeakSubjectsJson) 
                    ? JsonConvert.DeserializeObject<List<string>>(analysis.WeakSubjectsJson) ?? new List<string>()
                    : new List<string>(),
                Recommendations = analysis.Recommendations,
                AnalysisDate = analysis.AnalysisDate,
                AiModelUsed = analysis.AiModelUsed
            };
        }
    }
}
