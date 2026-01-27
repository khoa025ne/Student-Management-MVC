using DataAccess.Entities;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Implementations
{
    /// <summary>
    /// Enhanced ScoreService với AI Analysis và Email notification
    /// </summary>
    public class EnhancedScoreService : IScoreService
    {
        private readonly IScoreRepository _scoreRepo;
        private readonly IStudentService _studentService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IGeminiAIService _geminiAI;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EnhancedScoreService(
            IScoreRepository scoreRepo,
            IStudentService studentService,
            IEnrollmentService enrollmentService,
            IGeminiAIService geminiAI,
            IEmailService emailService,
            INotificationService notificationService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _scoreRepo = scoreRepo;
            _studentService = studentService;
            _enrollmentService = enrollmentService;
            _geminiAI = geminiAI;
            _emailService = emailService;
            _notificationService = notificationService;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<IEnumerable<Score>> GetAllAsync()
        {
            return await _scoreRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Score>> GetByStudentIdAsync(int studentId)
        {
            return await _scoreRepo.GetByStudentIdAsync(studentId);
        }

        public async Task<Score?> GetByIdAsync(int id)
        {
            return await _scoreRepo.GetByIdAsync(id);
        }

        /// <summary>
        /// Cập nhật điểm với AI Analysis và Email notification
        /// </summary>
        public async Task<Score> AddOrUpdateScoreAsync(int studentId, int courseId, double scoreValue)
        {
            var existingScore = await _scoreRepo.GetByStudentAndCourseAsync(studentId, courseId);
            
            Score result;
            if (existingScore != null)
            {
                existingScore.ScoreValue = scoreValue;
                result = await _scoreRepo.UpdateAsync(existingScore);
            }
            else
            {
                var newScore = new Score
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    ScoreValue = scoreValue
                };
                result = await _scoreRepo.AddAsync(newScore);
            }

            // Trigger background AI analysis và notifications
            _ = Task.Run(async () => await ProcessScoreUpdateBackgroundAsync(studentId, courseId, scoreValue));

            return result;
        }

        public async Task DeleteAsync(int id)
        {
            await _scoreRepo.DeleteAsync(id);
        }

        /// <summary>
        /// Background job: AI Analysis + Overall GPA + Email + Notification
        /// </summary>
        private async Task ProcessScoreUpdateBackgroundAsync(int studentId, int courseId, double scoreValue)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();
                var geminiAI = scope.ServiceProvider.GetRequiredService<IGeminiAIService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var student = await studentService.GetByIdAsync(studentId);
                if (student == null) return;

                // 1. Tính lại Overall GPA
                var newGPA = await studentService.CalculateOverallGPAAsync(studentId);

                // 2. Gọi AI phân tích
                var analysis = await geminiAI.AnalyzeStudentPerformanceAsync(studentId);
                
                if (analysis.Success)
                {
                    // Lưu kết quả AI vào database (cần tạo repository cho AcademicAnalysis)
                    // TODO: Implement AcademicAnalysisRepository
                    
                    // Gửi email phân tích AI
                    var strongSubjectsJson = System.Text.Json.JsonSerializer.Serialize(analysis.StrongSubjects ?? Array.Empty<string>());
                    var weakSubjectsJson = System.Text.Json.JsonSerializer.Serialize(analysis.WeakSubjects ?? Array.Empty<string>());
                    
                    await emailService.SendAIAnalysisNotificationAsync(
                        student.Email, 
                        student.FullName,
                        strongSubjectsJson,
                        weakSubjectsJson,
                        analysis.Recommendations ?? "Chưa có khuyến nghị",
                        newGPA);
                }

                // 3. Gửi email thông báo điểm mới
                var grade = CalculateGradeFromScore(scoreValue);
                await emailService.SendScoreNotificationAsync(
                    student.Email,
                    student.FullName,
                    "Môn học", // TODO: Lấy tên môn từ courseId
                    scoreValue,
                    grade);

                // 4. Tạo in-app notification
                await notificationService.CreateNotificationAsync(new Notification
                {
                    StudentId = studentId,
                    Title = "Điểm mới đã cập nhật",
                    Message = $"Bạn vừa nhận được điểm mới. GPA: {scoreValue:F2}, Grade: {grade}",
                    Type = "ScoreUpdate",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                });

                // 5. Kiểm tra cảnh báo học vụ
                await CheckAcademicWarningAsync(studentId, newGPA);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine($"Background AI Analysis Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Kiểm tra và gửi cảnh báo học vụ nếu cần
        /// </summary>
        private async Task CheckAcademicWarningAsync(int studentId, double overallGPA)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var enrollmentService = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

                var student = await studentService.GetByIdAsync(studentId);
                if (student == null) return;

                string? warningReason = null;

                // Check 1: Overall GPA < 2.0
                if (overallGPA < 2.0)
                {
                    warningReason = $"GPA tổng kết ({overallGPA:F2}) dưới 2.0 - Nguy cơ đình chỉ học tập";
                }
                else
                {
                    // Check 2: Có >= 2 môn F trong học kỳ gần nhất
                    var enrollments = await enrollmentService.GetByStudentAsync(studentId);
                    var recentFailures = enrollments
                        .Where(e => e.Grade == "F" && e.EnrollmentDate >= DateTime.Now.AddMonths(-6))
                        .Count();

                    if (recentFailures >= 2)
                    {
                        warningReason = $"Có {recentFailures} môn điểm F trong học kỳ gần đây";
                    }
                }

                if (!string.IsNullOrEmpty(warningReason))
                {
                    // FLOW 3: Gửi email cảnh báo cho sinh viên
                    await emailService.SendAcademicWarningAsync(
                        student.Email,
                        student.FullName,
                        overallGPA,
                        warningReason);

                    // FLOW 3: Gửi email cảnh báo cho Manager để theo dõi
                    await SendWarningToManagerAsync(emailService, student, overallGPA, warningReason);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Academic Warning Check Error: {ex.Message}");
            }
        }

        /// <summary>
        /// FLOW 3: Gửi email cảnh báo học vụ cho Manager
        /// </summary>
        private async Task SendWarningToManagerAsync(IEmailService emailService, Student student, double gpa, string reason)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();

                // Lấy danh sách Manager
                var managerRole = await roleRepository.GetByNameAsync("Manager");
                if (managerRole == null) return;

                var allUsers = await userRepository.GetAllAsync();
                var managers = allUsers.Where(u => u.RoleId == managerRole.RoleId).ToList();

                foreach (var manager in managers)
                {
                    // Gửi email thông báo cảnh báo cho Manager
                    await emailService.SendAcademicWarningAsync(
                        manager.Email,
                        $"Manager - Theo dõi SV: {student.FullName}",
                        gpa,
                        $"[CẢNH BÁO HỌC VỤ] Sinh viên {student.FullName} ({student.Email}) - {reason}"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send Warning to Manager Error: {ex.Message}");
            }
        }

        private string CalculateGradeFromScore(double score)
        {
            if (score >= 9.0) return "A+";
            if (score >= 8.5) return "A";
            if (score >= 8.0) return "B+";
            if (score >= 7.0) return "B";
            if (score >= 6.5) return "C+";
            if (score >= 5.5) return "C";
            if (score >= 5.0) return "D+";
            if (score >= 4.0) return "D";
            return "F";
        }
    }
}
