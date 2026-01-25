using Microsoft.Extensions.Options;
using Services.Interfaces;
using Services.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Implementations
{
    /// <summary>
    /// Gemini AI Service để phân tích học tập và đề xuất lộ trình
    /// </summary>
    public class GeminiAIService : IGeminiAIService
    {
        private readonly GeminiSettings _geminiSettings;
        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ICourseService _courseService;
        private readonly HttpClient _httpClient;

        public GeminiAIService(
            IOptions<GeminiSettings> geminiSettings,
            IStudentService studentService,
            IEnrollmentService enrollmentService,
            ICourseService courseService,
            HttpClient httpClient)
        {
            _geminiSettings = geminiSettings.Value;
            _studentService = studentService;
            _enrollmentService = enrollmentService;
            _courseService = courseService;
            _httpClient = httpClient;
        }

        public async Task<AcademicAnalysisResult> AnalyzeStudentPerformanceAsync(int studentId)
        {
            try
            {
                // 1. Lấy thông tin sinh viên
                var student = await _studentService.GetByIdAsync(studentId);
                if (student == null)
                {
                    return new AcademicAnalysisResult { Success = false, ErrorMessage = "Không tìm thấy sinh viên" };
                }

                // 2. Lấy tất cả các môn đã hoàn thành
                var enrollments = await _enrollmentService.GetByStudentAsync(studentId);
                var completedCourses = enrollments
                    .Where(e => e.TotalScore.HasValue && e.Grade != null)
                    .Select(e => new
                    {
                        CourseName = e.Class?.Course?.CourseName ?? "Unknown",
                        GPA = e.TotalScore ?? 0,
                        Grade = e.Grade ?? "N/A",
                        Credits = e.Class?.Course?.Credits ?? 0
                    })
                    .ToList();

                if (!completedCourses.Any())
                {
                    return new AcademicAnalysisResult
                    {
                        Success = true,
                        Recommendations = "Sinh viên chưa có điểm. Hãy cố gắng học tập tốt!",
                        StrongSubjects = Array.Empty<string>(),
                        WeakSubjects = Array.Empty<string>()
                    };
                }

                // 3. Chuẩn bị prompt cho Gemini
                var coursesJson = JsonSerializer.Serialize(completedCourses);
                var prompt = $@"
Bạn là cố vấn học tập chuyên nghiệp. Phân tích kết quả học tập sau:

- GPA tổng: {student.OverallGPA:F2}
- Các môn đã hoàn thành: {coursesJson}

Yêu cầu trả về JSON format:
{{
  ""strongSubjects"": [""Danh sách tên môn điểm cao (Grade A, B)""],
  ""weakSubjects"": [""Danh sách tên môn điểm thấp (Grade D, F)""],
  ""recommendations"": ""Khuyến nghị cải thiện (tối đa 200 từ)""
}}

Chỉ trả về JSON, không thêm text khác.";

                // 4. Gọi Gemini API
                var response = await CallGeminiAPIAsync(prompt);

                // 5. Parse response
                if (string.IsNullOrEmpty(response))
                {
                    return UseFallbackAnalysis(completedCourses);
                }

                // Làm sạch response (loại bỏ markdown code block nếu có)
                response = response.Trim();
                if (response.StartsWith("```json"))
                {
                    response = response.Substring(7);
                }
                if (response.StartsWith("```"))
                {
                    response = response.Substring(3);
                }
                if (response.EndsWith("```"))
                {
                    response = response.Substring(0, response.Length - 3);
                }
                response = response.Trim();

                var result = JsonSerializer.Deserialize<AcademicAnalysisResult>(response);
                if (result != null)
                {
                    result.Success = true;
                    return result;
                }

                return UseFallbackAnalysis(completedCourses);
            }
            catch (Exception ex)
            {
                // Fallback nếu AI fail
                var enrollments = await _enrollmentService.GetByStudentAsync(studentId);
                var completedCourses = enrollments
                    .Where(e => e.TotalScore.HasValue && e.Grade != null)
                    .Select(e => new
                    {
                        CourseName = e.Class?.Course?.CourseName ?? "Unknown",
                        Grade = e.Grade ?? "N/A"
                    })
                    .ToList();

                return UseFallbackAnalysis(completedCourses);
            }
        }

        public async Task<LearningPathResult> GenerateLearningPathAsync(int studentId, int semesterId)
        {
            try
            {
                // 1. Lấy thông tin sinh viên
                var student = await _studentService.GetByIdAsync(studentId);
                if (student == null)
                {
                    return new LearningPathResult { Success = false, ErrorMessage = "Không tìm thấy sinh viên" };
                }

                // 2. Lấy các môn đã hoàn thành
                var enrollments = await _enrollmentService.GetByStudentAsync(studentId);
                var passedCourseIds = enrollments
                    .Where(e => e.IsPassed && e.Class?.CourseId != null)
                    .Select(e => e.Class.CourseId)
                    .ToList();

                // 3. Lấy tất cả các môn học
                var allCourses = await _courseService.GetAllAsync();
                var availableCourses = allCourses
                    .Where(c => !passedCourseIds.Contains(c.CourseId))
                    .Where(c => !c.PrerequisiteCourseId.HasValue || passedCourseIds.Contains(c.PrerequisiteCourseId.Value))
                    .ToList();

                // 4. Lấy phân tích điểm mạnh/yếu
                var analysis = await AnalyzeStudentPerformanceAsync(studentId);

                // 5. Chuẩn bị prompt cho Gemini
                var coursesJson = JsonSerializer.Serialize(availableCourses.Select(c => new
                {
                    c.CourseName,
                    c.CourseCode,
                    c.Credits,
                    c.Major
                }));

                var prompt = $@"
Bạn là cố vấn học tập AI. Gợi ý lộ trình học tập dựa vào:

- GPA hiện tại: {student.OverallGPA:F2}
- Môn mạnh: {string.Join(", ", analysis.StrongSubjects)}
- Môn yếu: {string.Join(", ", analysis.WeakSubjects)}
- Chuyên ngành: {student.Major}
- Các môn chưa học: {coursesJson}

Hãy gợi ý 3-4 môn PHÙ HỢP NHẤT cho kỳ tới. Trả về JSON:
{{
  ""recommendedCourses"": [
    {{
      ""courseName"": ""Tên môn"",
      ""courseCode"": ""Mã môn"",
      ""priority"": 1-4 (1 cao nhất),
      ""reason"": ""Lý do nên học môn này (tối đa 100 từ)""
    }}
  ],
  ""overallStrategy"": ""Chiến lược học tập tổng quát (tối đa 200 từ)"",
  ""warnings"": [""Các cảnh báo nếu có""]
}}

Chỉ trả về JSON, không thêm text khác.";

                // 6. Gọi Gemini API
                var response = await CallGeminiAPIAsync(prompt);

                if (string.IsNullOrEmpty(response))
                {
                    return UseFallbackLearningPath(availableCourses);
                }

                // Làm sạch response
                response = response.Trim();
                if (response.StartsWith("```json")) response = response.Substring(7);
                if (response.StartsWith("```")) response = response.Substring(3);
                if (response.EndsWith("```")) response = response.Substring(0, response.Length - 3);
                response = response.Trim();

                var result = JsonSerializer.Deserialize<LearningPathResult>(response);
                if (result != null)
                {
                    result.Success = true;
                    return result;
                }

                return UseFallbackLearningPath(availableCourses);
            }
            catch (Exception ex)
            {
                var allCourses = await _courseService.GetAllAsync();
                return UseFallbackLearningPath(allCourses.Take(4).ToList());
            }
        }

        /// <summary>
        /// Gọi Gemini API
        /// </summary>
        private async Task<string> CallGeminiAPIAsync(string prompt)
        {
            try
            {
                var requestUrl = $"{_geminiSettings.ApiEndpoint}/models/{_geminiSettings.Model}:generateContent?key={_geminiSettings.ApiKey}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(requestUrl, httpContent);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseBody);

                // Parse Gemini response format
                var text = jsonDoc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return text ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Gemini API Error: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Fallback analysis khi AI fail
        /// </summary>
        private AcademicAnalysisResult UseFallbackAnalysis(dynamic completedCourses)
        {
            var coursesList = (IEnumerable<dynamic>)completedCourses;
            
            var strong = coursesList
                .Where(c => c.Grade == "A" || c.Grade == "A+" || c.Grade == "B" || c.Grade == "B+")
                .Select(c => (string)c.CourseName)
                .ToArray();

            var weak = coursesList
                .Where(c => c.Grade == "D" || c.Grade == "F")
                .Select(c => (string)c.CourseName)
                .ToArray();

            return new AcademicAnalysisResult
            {
                Success = true,
                StrongSubjects = strong,
                WeakSubjects = weak,
                Recommendations = weak.Any()
                    ? "Tập trung ôn tập các môn yếu. Tham gia học bổ trợ nếu cần. Sắp xếp thời gian học tập hợp lý hơn."
                    : "Bạn đang học tốt! Hãy duy trì và phát huy thêm."
            };
        }

        /// <summary>
        /// Fallback learning path khi AI fail
        /// </summary>
        private LearningPathResult UseFallbackLearningPath(dynamic courses)
        {
            var coursesList = ((IEnumerable<dynamic>)courses).ToList();
            
            var recommendedCourses = coursesList
                .Take(4)
                .Select((c, index) => new RecommendedCourse
                {
                    CourseName = c.CourseName,
                    CourseCode = c.CourseCode,
                    Priority = index + 1,
                    Reason = "Môn học phù hợp với chuyên ngành và tiến độ học tập của bạn."
                })
                .ToArray();

            return new LearningPathResult
            {
                Success = true,
                RecommendedCourses = recommendedCourses,
                OverallStrategy = "Tập trung vào các môn cơ sở để xây dựng nền tảng vững chắc.",
                Warnings = Array.Empty<string>()
            };
        }
    }
}
