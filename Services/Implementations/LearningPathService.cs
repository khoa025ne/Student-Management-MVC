using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Services.Implementations
{
    public class LearningPathService : ILearningPathService
    {
        private readonly ILearningPathRepository _learningPathRepo;
        private readonly IStudentService _studentService;
        private readonly ISemesterService _semesterService;
        private readonly IGeminiAIService _geminiAIService;

        public LearningPathService(
            ILearningPathRepository learningPathRepo, 
            IStudentService studentService, 
            ISemesterService semesterService,
            IGeminiAIService geminiAIService)
        {
            _learningPathRepo = learningPathRepo;
            _studentService = studentService;
            _semesterService = semesterService;
            _geminiAIService = geminiAIService;
        }

        public async Task<LearningPathRecommendation?> GetLatestRecommendationAsync(int userId)
        {
            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null) return null;
            return await _learningPathRepo.GetLatestByStudentAsync(student.StudentId);
        }

        public async Task<LearningPathRecommendation> GenerateRecommendationAsync(int userId)
        {
            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null) throw new Exception("Student not found");

            var activeSemester = await _semesterService.GetActiveAsync();
            var semesterId = activeSemester?.SemesterId ?? 1;
            
            // FLOW 3: Sử dụng Gemini AI thật thay vì mock data
            try
            {
                var aiResult = await _geminiAIService.GenerateLearningPathAsync(
                    student.StudentId, 
                    semesterId
                );

                if (aiResult.Success && aiResult.RecommendedCourses?.Length > 0)
                {
                    // Chuyển đổi từ AI result sang JSON
                    var coursesJson = JsonConvert.SerializeObject(
                        aiResult.RecommendedCourses.Select(c => new
                        {
                            Name = c.CourseName,
                            Code = c.CourseCode,
                            Priority = c.Priority,
                            Reason = c.Reason
                        }).ToArray()
                    );

                    var warningsJson = JsonConvert.SerializeObject(aiResult.Warnings);

                    var recommendation = new LearningPathRecommendation
                    {
                        StudentId = student.StudentId,
                        SemesterId = activeSemester?.SemesterId ?? 1,
                        RecommendationDate = DateTime.Now,
                        OverallStrategy = aiResult.OverallStrategy,
                        RecommendedCoursesJson = coursesJson,
                        WarningsJson = warningsJson,
                        AiModelUsed = "Gemini-AI" // Thực tế sử dụng Gemini
                    };

                    return await _learningPathRepo.AddAsync(recommendation);
                }
                else
                {
                    // AI failed → Fallback to basic recommendation
                    return await GenerateFallbackRecommendationAsync(student.StudentId, activeSemester?.SemesterId ?? 1);
                }
            }
            catch (Exception ex)
            {
                // Exception → Fallback
                Console.WriteLine($"[LearningPathService] AI Error: {ex.Message}. Falling back to basic recommendation.");
                return await GenerateFallbackRecommendationAsync(student.StudentId, activeSemester?.SemesterId ?? 1);
            }
        }

        /// <summary>
        /// Fallback recommendation khi AI không khả dụng
        /// </summary>
        private async Task<LearningPathRecommendation> GenerateFallbackRecommendationAsync(int studentId, int semesterId)
        {
            var recommendation = new LearningPathRecommendation
            {
                StudentId = studentId,
                SemesterId = semesterId,
                RecommendationDate = DateTime.Now,
                OverallStrategy = "Tập trung cải thiện các môn cơ sở ngành. (Hệ thống AI tạm thời không khả dụng)",
                RecommendedCoursesJson = "[{\"Name\":\"Lập trình .NET nâng cao\",\"Code\":\"NET102\",\"Priority\":1,\"Reason\":\"Môn cốt lõi cho ngành CNTT\"}]",
                WarningsJson = "[\"Vui lòng tham khảo ý kiến cố vấn học tập để có lộ trình phù hợp.\"]",
                AiModelUsed = "Fallback-v1"
            };

            return await _learningPathRepo.AddAsync(recommendation);
        }
    }
}
