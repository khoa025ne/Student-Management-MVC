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
                        MidtermScore = e.MidtermScore ?? 0,
                        FinalScore = e.FinalScore ?? 0,
                        TotalScore = e.TotalScore ?? 0,
                        Grade = e.Grade ?? "N/A",
                        Credits = e.Class?.Course?.Credits ?? 0,
                        Improvement = (e.FinalScore ?? 0) - (e.MidtermScore ?? 0)
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

                // 3. Chuẩn bị prompt cho Gemini với thông tin chi tiết
                var coursesJson = JsonSerializer.Serialize(completedCourses);
                var avgImprovement = completedCourses.Average(c => c.Improvement);
                var totalCredits = completedCourses.Sum(c => c.Credits);
                
                var prompt = $@"
Bạn là cố vấn học tập AI chuyên nghiệp. Phân tích CHI TIẾT kết quả học tập sau:

📊 THÔNG TIN TỔNG QUAN:
- GPA tổng kết: {student.OverallGPA:F2}
- Số môn đã hoàn thành: {completedCourses.Count}
- Tổng tín chỉ: {totalCredits}
- Xu hướng cải thiện TB: {avgImprovement:+0.0;-0.0;0} điểm (Final - Midterm)

📚 CHI TIẾT TỪNG MÔN HỌC:
{coursesJson}

YÊU CẦU PHÂN TÍCH:

1️⃣ ĐIỂM MẠNH (strongSubjects):
   - Liệt kê các môn có Grade A+, A, B+ (điểm cao)
   - Ưu tiên môn có xu hướng tiến bộ (FinalScore > MidtermScore)
   - Format: ""[Tên môn] (Điểm TB: X.X, Tiến bộ: +Y.Y)""

2️⃣ ĐIỂM YẾU (weakSubjects):
   - Liệt kê các môn có Grade D, F hoặc điểm < 5.0
   - Chú ý môn tụt điểm (FinalScore < MidtermScore)
   - Format: ""[Tên môn] (Điểm TB: X.X, Xu hướng: -Y.Y)""

3️⃣ KHUYẾN NGHỊ (recommendations):
   - So sánh ĐIỂM GIỮA KÌ vs ĐIỂM CUỐI KÌ: Phân tích xu hướng học tập
   - Đánh giá SỰ ỔN ĐỊNH: Sinh viên học đều hay chỉ tốt ở một số môn?
   - Đề xuất cải thiện: Cần tập trung ôn tập trước thi, rèn luyện thường xuyên, hay phân bổ thời gian đều hơn
   - Lưu ý môn có điểm giữa kì cao nhưng cuối kì thấp (suy giảm)
   - Tối đa 250 từ, rõ ràng, thực tế

⚠️ LƯU Ý QUAN TRỌNG:
- Phân tích dựa trên DỮ LIỆU THỰC TẾ, không chung chung
- Nhắc đến TÊN MÔN CỤ THỂ trong recommendations
- So sánh xu hướng midterm vs final để đánh giá khả năng duy trì

Trả về JSON format (KHÔNG thêm markdown ```json):
{{
  ""strongSubjects"": [""Tên môn (Điểm: X.X, Tiến bộ: +Y)""],
  ""weakSubjects"": [""Tên môn (Điểm: X.X, Xu hướng: -Y)""],
  ""recommendations"": ""Phân tích chi tiết với số liệu cụ thể""
}}";

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
                // Fallback nếu AI fail - dùng dữ liệu đầy đủ
                var enrollments = await _enrollmentService.GetByStudentAsync(studentId);
                var completedCourses = enrollments
                    .Where(e => e.TotalScore.HasValue && e.Grade != null)
                    .Select(e => new
                    {
                        CourseName = e.Class?.Course?.CourseName ?? "Unknown",
                        MidtermScore = e.MidtermScore ?? 0,
                        FinalScore = e.FinalScore ?? 0,
                        TotalScore = e.TotalScore ?? 0,
                        Grade = e.Grade ?? "N/A",
                        Credits = e.Class?.Course?.Credits ?? 0,
                        Improvement = (e.FinalScore ?? 0) - (e.MidtermScore ?? 0)
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
                .Select(c => $"{c.CourseName} (Điểm: {c.TotalScore:F1}, Tiến bộ: {c.Improvement:+0.0;-0.0;0})")
                .ToArray();

            var weak = coursesList
                .Where(c => c.Grade == "D" || c.Grade == "F" || c.TotalScore < 5.0)
                .Select(c => $"{c.CourseName} (Điểm: {c.TotalScore:F1}, Xu hướng: {c.Improvement:+0.0;-0.0;0})")
                .ToArray();

            var avgImprovement = coursesList.Average(c => (double)c.Improvement);
            var decliningCourses = coursesList.Where(c => c.Improvement < -1.0).ToList();
            var improvingCourses = coursesList.Where(c => c.Improvement > 1.0).ToList();
            
            var recommendations = "";
            
            if (avgImprovement > 0.5)
            {
                recommendations = $"🎯 Xu hướng tích cực: Bạn đã cải thiện {avgImprovement:F1} điểm từ giữa kì đến cuối kì. ";
            }
            else if (avgImprovement < -0.5)
            {
                recommendations = $"⚠️ Cảnh báo: Điểm cuối kì giảm {Math.Abs(avgImprovement):F1} điểm so với giữa kì. ";
            }
            
            if (decliningCourses.Any())
            {
                var declining = string.Join(", ", decliningCourses.Select(c => c.CourseName).Take(2));
                recommendations += $"Cần chú ý: {declining} có xu hướng suy giảm. Hãy ôn tập thường xuyên hơn, không chỉ tập trung trước kỳ thi. ";
            }
            
            if (improvingCourses.Any())
            {
                var improving = string.Join(", ", improvingCourses.Select(c => c.CourseName).Take(2));
                recommendations += $"Tiến bộ tốt ở: {improving}. Hãy duy trì phương pháp học này! ";
            }

            if (weak.Any())
            {
                recommendations += $"Tập trung cải thiện các môn yếu. Tham gia học bổ trợ, lập nhóm học tập, và phân bổ thời gian đều cho tất cả các môn.";
            }
            else if (strong.Length > weak.Length * 2)
            {
                recommendations += "Bạn đang học tốt! Hãy duy trì sự ổn định và phát huy thêm ở tất cả các môn.";
            }
            else
            {
                recommendations += "Kết quả chưa đồng đều giữa các môn. Hãy phân bổ thời gian học tập cân bằng hơn để tất cả môn đều đạt điểm cao.";
            }

            return new AcademicAnalysisResult
            {
                Success = true,
                StrongSubjects = strong,
                WeakSubjects = weak,
                Recommendations = recommendations.Trim()
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
