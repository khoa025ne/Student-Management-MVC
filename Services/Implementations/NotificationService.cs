using DataAccess.Entities;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IStudentService _studentService;
        private readonly IEmailService _emailService;

        public NotificationService(
            INotificationRepository notificationRepo, 
            IStudentService studentService,
            IEmailService emailService)
        {
            _notificationRepo = notificationRepo;
            _studentService = studentService;
            _emailService = emailService;
        }

        public async Task<IEnumerable<Notification>> GetAllAsync()
        {
            return await _notificationRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Notification>> GetMyNotificationsAsync(int userId)
        {
            // Find Student for this user
            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                // If not student, maybe just return nothing or generic notifications if we support non-student notifs
                return Enumerable.Empty<Notification>();
            }
            return await _notificationRepo.GetByStudentIdAsync(student.StudentId);
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _notificationRepo.GetByIdAsync(id);
        }

        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            return await _notificationRepo.AddAsync(notification);
        }

        public async Task MarkAsReadAsync(int id)
        {
            await _notificationRepo.MarkAsReadAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            await _notificationRepo.DeleteAsync(id);
        }

        // ═══════════════════════════════════════════════════════════════
        // EVENT-DRIVEN NOTIFICATION METHODS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Event: Score Update - Gửi thông báo điểm mới
        /// </summary>
        public async Task SendScoreUpdateNotificationAsync(int studentId, string courseName, double score, string grade)
        {
            var student = await _studentService.GetByIdAsync(studentId);
            if (student?.User == null) return;

            // In-app notification
            await CreateNotificationAsync(new Notification
            {
                StudentId = studentId,
                Title = "📊 Điểm mới đã cập nhật",
                Message = $"Môn {courseName}: {score:F1} điểm ({grade})",
                Type = "Score Update",
                IsRead = false,
                CreatedAt = DateTime.Now,
                Link = "/Students/MyGrades"
            });

            // Email notification
            await _emailService.SendScoreNotificationAsync(
                student.User.Email,
                student.User.FullName,
                courseName,
                score,
                grade
            );
        }

        /// <summary>
        /// Event: Achievement - Chúc mừng đạt điểm A/A+
        /// </summary>
        public async Task SendAchievementNotificationAsync(int studentId, string courseName, string grade)
        {
            var student = await _studentService.GetByIdAsync(studentId);
            if (student?.User == null) return;

            var achievementMessage = grade == "A+" 
                ? $"🏆 Xuất sắc! Bạn đạt điểm {grade} môn {courseName}!" 
                : $"🎉 Chúc mừng! Bạn đạt điểm {grade} môn {courseName}!";

            // In-app notification với style đặc biệt
            await CreateNotificationAsync(new Notification
            {
                StudentId = studentId,
                Title = "🎓 Thành tích mới!",
                Message = achievementMessage,
                Type = "Achievement",
                IsRead = false,
                CreatedAt = DateTime.Now,
                Link = "/Students/MyGrades"
            });

            // Email chúc mừng
            var emailSubject = $"🎉 Chúc mừng! Bạn đạt điểm {grade}";
            var emailBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);'>
                        <div style='background: white; padding: 30px; border-radius: 10px;'>
                            <h2 style='color: #667eea; text-align: center;'>🏆 THÀNH TÍCH MỚI!</h2>
                            <p>Xin chào <strong>{student.User.FullName}</strong>,</p>
                            <div style='background: #f0f4ff; padding: 20px; border-radius: 8px; margin: 20px 0; text-align: center;'>
                                <h3 style='color: #667eea; margin: 0;'>{courseName}</h3>
                                <h1 style='color: #28a745; font-size: 48px; margin: 10px 0;'>{grade}</h1>
                                <p style='color: #666; margin: 0;'>Kết quả xuất sắc!</p>
                            </div>
                            <p style='color: #555;'>
                                Chúc mừng bạn đã đạt được thành tích tuyệt vời! Đây là minh chứng cho sự nỗ lực và cống hiến của bạn.
                                Hãy tiếp tục phát huy và duy trì phong độ này!
                            </p>
                            <p style='text-align: center; margin: 30px 0;'>
                                <a href='#' style='display: inline-block; padding: 12px 30px; background-color: #667eea; color: white; text-decoration: none; border-radius: 5px;'>
                                    Xem Bảng Điểm
                                </a>
                            </p>
                        </div>
                    </div>
                </body>
                </html>
            ";
            await _emailService.SendEmailAsync(student.User.Email, emailSubject, emailBody);
        }

        /// <summary>
        /// Event: Performance Alert - Cảnh báo môn yếu (D/F)
        /// </summary>
        public async Task SendPerformanceAlertNotificationAsync(int studentId, string courseName, string grade, string reason)
        {
            var student = await _studentService.GetByIdAsync(studentId);
            if (student?.User == null) return;

            var alertLevel = grade == "F" ? "🔴 Nghiêm trọng" : "⚠️ Cảnh báo";
            var alertMessage = grade == "F"
                ? $"Môn {courseName} điểm {grade} - Cần học lại!"
                : $"Môn {courseName} điểm {grade} - Cần cải thiện!";

            // In-app notification
            await CreateNotificationAsync(new Notification
            {
                StudentId = studentId,
                Title = $"{alertLevel} - Kết quả học tập",
                Message = alertMessage,
                Type = "Performance Alert",
                IsRead = false,
                CreatedAt = DateTime.Now,
                Link = "/Students/MyGrades"
            });

            // Email cảnh báo
            var emailSubject = $"⚠️ Cảnh báo: Môn {courseName} cần chú ý";
            var emailBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background: #fff3cd; border-left: 5px solid #ff6b35; padding: 20px; border-radius: 5px;'>
                            <h2 style='color: #ff6b35; margin-top: 0;'>⚠️ Cảnh báo kết quả học tập</h2>
                            <p>Xin chào <strong>{student.User.FullName}</strong>,</p>
                            <div style='background: white; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                                <h3 style='color: #dc3545; margin: 0;'>{courseName}</h3>
                                <p style='font-size: 24px; font-weight: bold; color: #dc3545; margin: 10px 0;'>Điểm: {grade}</p>
                                <p style='color: #666; margin: 0;'>{reason}</p>
                            </div>
                            <h4 style='color: #ff6b35;'>💡 Gợi ý cải thiện:</h4>
                            <ul style='color: #555;'>
                                <li>Tham gia lớp học bổ trợ</li>
                                <li>Gặp giảng viên để được tư vấn</li>
                                <li>Lập nhóm học tập với bạn bè</li>
                                <li>Xem lại tài liệu và bài giảng</li>
                            </ul>
                            <p style='text-align: center; margin: 20px 0;'>
                                <a href='#' style='display: inline-block; padding: 12px 30px; background-color: #ff6b35; color: white; text-decoration: none; border-radius: 5px;'>
                                    Xem Phân Tích Chi Tiết
                                </a>
                            </p>
                        </div>
                    </div>
                </body>
                </html>
            ";
            await _emailService.SendEmailAsync(student.User.Email, emailSubject, emailBody);
        }

        /// <summary>
        /// Event: Learning Path - Gợi ý môn học từ AI
        /// </summary>
        public async Task SendLearningPathNotificationAsync(int studentId, string[] recommendedCourses)
        {
            var student = await _studentService.GetByIdAsync(studentId);
            if (student?.User == null) return;

            var coursesText = string.Join(", ", recommendedCourses.Take(3));
            
            // In-app notification
            await CreateNotificationAsync(new Notification
            {
                StudentId = studentId,
                Title = "💡 AI gợi ý lộ trình học tập",
                Message = $"Các môn phù hợp: {coursesText}",
                Type = "Learning Path",
                IsRead = false,
                CreatedAt = DateTime.Now,
                Link = "/Students/Dashboard" // Hoặc trang Learning Path nếu có
            });

            // Email gợi ý chi tiết
            var coursesList = string.Join("", recommendedCourses.Select(c => $"<li style='margin: 8px 0;'>{c}</li>"));
            
            var emailSubject = "💡 AI đã gợi ý lộ trình học tập cho bạn";
            var emailBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; border-radius: 10px; color: white;'>
                            <h2 style='margin: 0;'>🤖 Lộ Trình Học Tập AI</h2>
                            <p style='margin: 10px 0 0 0; opacity: 0.9;'>Dựa trên kết quả học tập của bạn</p>
                        </div>
                        <div style='background: white; padding: 30px; border: 1px solid #e0e0e0; border-radius: 0 0 10px 10px;'>
                            <p>Xin chào <strong>{student.User.FullName}</strong>,</p>
                            <p>Hệ thống AI đã phân tích kết quả học tập và gợi ý các môn học phù hợp cho kỳ tới:</p>
                            
                            <div style='background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
                                <h3 style='color: #667eea; margin-top: 0;'>📚 Môn học được gợi ý:</h3>
                                <ul style='color: #555; line-height: 1.8;'>
                                    {coursesList}
                                </ul>
                            </div>

                            <div style='background: #e3f2fd; padding: 15px; border-radius: 8px; border-left: 4px solid #2196f3;'>
                                <strong style='color: #1976d2;'>💬 Tại sao nên học các môn này?</strong>
                                <p style='color: #555; margin: 10px 0 0 0;'>
                                    Các môn này phù hợp với điểm mạnh của bạn và sẽ giúp bạn phát triển kỹ năng cần thiết cho ngành học.
                                    Đồng thời, chúng cũng là nền tảng cho các môn chuyên sâu sau này.
                                </p>
                            </div>

                            <p style='text-align: center; margin: 30px 0;'>
                                <a href='#' style='display: inline-block; padding: 12px 30px; background-color: #667eea; color: white; text-decoration: none; border-radius: 5px;'>
                                    Xem Chi Tiết Gợi Ý
                                </a>
                            </p>
                        </div>
                    </div>
                </body>
                </html>
            ";
            await _emailService.SendEmailAsync(student.User.Email, emailSubject, emailBody);
        }
    }
}
