using DataAccess.Entities;
using DataAccess.DAO;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models;

namespace Services.Implementations
{
    public class AcademicAnalysisService : IAcademicAnalysisService
    {
        private readonly IAcademicAnalysisRepository _analysisRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IScoreRepository _scoreRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IGeminiAIService _geminiAIService;

        public AcademicAnalysisService(
            IAcademicAnalysisRepository analysisRepository,
            IStudentRepository studentRepository,
            IScoreRepository scoreRepository,
            IEnrollmentRepository enrollmentRepository,
            IGeminiAIService geminiAIService)
        {
            _analysisRepository = analysisRepository;
            _studentRepository = studentRepository;
            _scoreRepository = scoreRepository;
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
            var analyses = await _analysisRepository.GetByStudentIdAsync(studentId);
            return analyses.Select(MapToDto);
        }

        public async Task<AcademicAnalysisDto?> CreateAnalysisAsync(string studentId, string analysisType)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
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
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student == null)
                    return null;

                // Lấy điểm của sinh viên
                var enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId);
                var scores = new List<Score>();

                foreach (var enrollment in enrollments)
                {
                    var enrollmentScores = await _scoreRepository.GetByEnrollmentIdAsync(enrollment.EnrollmentId);
                    scores.AddRange(enrollmentScores);
                }

                // Tính toán GPA và tạo phân tích
                var gpaData = CalculateGPA(scores);
                var analysisPrompt = $@"
                Sinh viên: {student.FullName}
                GPA hiện tại: {gpaData.GPA:F2}
                Số môn đã học: {gpaData.CompletedCourses}
                Điểm trung bình các môn: {string.Join(", ", gpaData.CourseAverages.Select(x => $"{x.Key}: {x.Value:F1}"))}
                
                Hãy phân tích học lực và đưa ra khuyến nghị cụ thể để cải thiện.";

                var aiAnalysis = await _geminiAIService.GenerateAnalysisAsync(analysisPrompt);

                var analysis = new AcademicAnalysis
                {
                    StudentId = studentId,
                    AnalysisType = "GPA Analysis",
                    Content = $"GPA hiện tại: {gpaData.GPA:F2}\\nSố môn hoàn thành: {gpaData.CompletedCourses}",
                    Recommendations = aiAnalysis,
                    GeneratedDate = DateTime.Now,
                    IsActive = true
                };

                var savedAnalysis = await _analysisRepository.CreateAsync(analysis);
                return MapToDto(savedAnalysis);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating GPA analysis: {ex.Message}", ex);
            }
        }

        public async Task<AcademicAnalysisDto?> GeneratePerformanceTrendAnalysisAsync(string studentId)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student == null)
                    return null;

                // Logic phân tích xu hướng học tập
                var enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId);
                var performanceData = new List<(string Course, double Average, DateTime Date)>();

                foreach (var enrollment in enrollments)
                {
                    var scores = await _scoreRepository.GetByEnrollmentIdAsync(enrollment.EnrollmentId);
                    if (scores.Any())
                    {
                        var average = scores.Average(s => s.ScoreValue);
                        performanceData.Add((enrollment.Class?.Course?.CourseName ?? "Unknown", average, enrollment.EnrollmentDate));
                    }
                }

                var trend = AnalyzeTrend(performanceData);
                var aiAnalysis = await _geminiAIService.GenerateAnalysisAsync(
                    $"Phân tích xu hướng học tập của sinh viên {student.FullName}: {trend}");

                var analysis = new AcademicAnalysis
                {
                    StudentId = studentId,
                    AnalysisType = "Performance Trend",
                    Content = trend,
                    Recommendations = aiAnalysis,
                    GeneratedDate = DateTime.Now,
                    IsActive = true
                };

                var savedAnalysis = await _analysisRepository.CreateAsync(analysis);
                return MapToDto(savedAnalysis);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating performance trend analysis: {ex.Message}", ex);
            }
        }

        public async Task<AcademicAnalysisDto?> GenerateLearningPathAnalysisAsync(string studentId)
        {
            try
            {
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student == null)
                    return null;

                // Logic phân tích lộ trình học tập
                var enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId);
                var completedCourses = enrollments.Where(e => e.Status == "Completed").Select(e => e.Class?.Course?.CourseName).ToList();
                var inProgressCourses = enrollments.Where(e => e.Status == "Active").Select(e => e.Class?.Course?.CourseName).ToList();

                var pathAnalysis = $"Đã hoàn thành: {completedCourses.Count} môn\\nĐang học: {inProgressCourses.Count} môn";
                var aiRecommendations = await _geminiAIService.GenerateAnalysisAsync(
                    $"Tạo lộ trình học tập cho sinh viên {student.FullName}. Đã hoàn thành: {string.Join(", ", completedCourses)}");

                var analysis = new AcademicAnalysis
                {
                    StudentId = studentId,
                    AnalysisType = "Learning Path",
                    Content = pathAnalysis,
                    Recommendations = aiRecommendations,
                    GeneratedDate = DateTime.Now,
                    IsActive = true
                };

                var savedAnalysis = await _analysisRepository.CreateAsync(analysis);
                return MapToDto(savedAnalysis);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating learning path analysis: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<AcademicAnalysisDto>> GetStudentsAtRiskAsync()
        {
            var allAnalyses = await _analysisRepository.GetAllAsync();
            
            // Logic xác định sinh viên có nguy cơ
            var riskAnalyses = allAnalyses.Where(a => 
                a.Content.Contains("GPA") && 
                ExtractGPA(a.Content) < 2.0).ToList();

            return riskAnalyses.Select(MapToDto);
        }

        public async Task<bool> UpdateAnalysisStatusAsync(int analysisId, string status)
        {
            var analysis = await _analysisRepository.GetByIdAsync(analysisId);
            if (analysis == null)
                return false;

            // Update status logic would go here
            analysis.IsActive = status == "Active";
            await _analysisRepository.UpdateAsync(analysis);
            return true;
        }

        private AcademicAnalysisDto MapToDto(AcademicAnalysis analysis)
        {
            return new AcademicAnalysisDto
            {
                AnalysisId = analysis.AnalysisId,
                StudentId = analysis.StudentId,
                StudentName = analysis.Student?.FullName,
                AnalysisType = analysis.AnalysisType,
                Content = analysis.Content,
                Recommendations = analysis.Recommendations,
                GeneratedDate = analysis.GeneratedDate,
                IsActive = analysis.IsActive,
                CurrentGPA = ExtractGPA(analysis.Content),
                RiskLevel = DetermineRiskLevel(analysis.Content),
                Status = analysis.IsActive ? "Active" : "Inactive"
            };
        }

        private (double GPA, int CompletedCourses, Dictionary<string, double> CourseAverages) CalculateGPA(List<Score> scores)
        {
            if (!scores.Any())
                return (0.0, 0, new Dictionary<string, double>());

            var groupedScores = scores.GroupBy(s => s.Enrollment?.Class?.Course?.CourseName ?? "Unknown");
            var courseAverages = new Dictionary<string, double>();
            var totalGrade = 0.0;
            var courseCount = 0;

            foreach (var courseGroup in groupedScores)
            {
                var average = courseGroup.Average(s => s.ScoreValue);
                courseAverages[courseGroup.Key] = average;
                totalGrade += average;
                courseCount++;
            }

            var gpa = courseCount > 0 ? totalGrade / courseCount : 0.0;
            return (gpa, courseCount, courseAverages);
        }

        private string AnalyzeTrend(List<(string Course, double Average, DateTime Date)> performanceData)
        {
            if (performanceData.Count < 2)
                return "Chưa đủ dữ liệu để phân tích xu hướng";

            var orderedData = performanceData.OrderBy(x => x.Date).ToList();
            var firstHalf = orderedData.Take(orderedData.Count / 2).Average(x => x.Average);
            var secondHalf = orderedData.Skip(orderedData.Count / 2).Average(x => x.Average);

            if (secondHalf > firstHalf + 0.5)
                return "Xu hướng tích cực: Điểm số đang cải thiện theo thời gian";
            else if (firstHalf > secondHalf + 0.5)
                return "Xu hướng tiêu cực: Điểm số đang giảm theo thời gian";
            else
                return "Xu hướng ổn định: Điểm số duy trì ổn định";
        }

        private double ExtractGPA(string content)
        {
            var gpaMatch = System.Text.RegularExpressions.Regex.Match(content, @"GPA[:\s]+(\d+\.?\d*)");
            if (gpaMatch.Success && double.TryParse(gpaMatch.Groups[1].Value, out double gpa))
                return gpa;
            return 0.0;
        }

        private string DetermineRiskLevel(string content)
        {
            var gpa = ExtractGPA(content);
            if (gpa < 1.5) return "High";
            if (gpa < 2.0) return "Medium";
            if (gpa < 2.5) return "Low";
            return "None";
        }
    }
}