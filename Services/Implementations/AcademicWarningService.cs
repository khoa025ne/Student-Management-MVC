using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Services.Interfaces;
using Services.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementations
{
    /// <summary>
    /// Service xử lý cảnh báo học vụ - Business Logic Layer
    /// Tách biệt khỏi BackgroundService host ở Presentation layer
    /// </summary>
    public class AcademicWarningService : IAcademicWarningService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<AcademicWarningService> _logger;

        // Action delegate để gửi SignalR notification (được inject từ MVC layer)
        public Func<int, NotificationMessageDto, Task>? OnSendRealTimeNotification { get; set; }

        public AcademicWarningService(
            IStudentRepository studentRepository,
            IEnrollmentRepository enrollmentRepository,
            INotificationRepository notificationRepository,
            IEmailService emailService,
            ILogger<AcademicWarningService> logger)
        {
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
            _notificationRepository = notificationRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<int> CheckAndSendAcademicWarningsAsync()
        {
            _logger.LogInformation("⏰ Checking academic warnings...");

            var allStudents = await _studentRepository.GetAllAsync();
            int warningCount = 0;

            foreach (var student in allStudents)
            {
                try
                {
                    // 1. Kiểm tra GPA < 2.0
                    if (student.OverallGPA > 0 && student.OverallGPA < 2.0)
                    {
                        await SendGPAWarningAsync(student.StudentId);
                        warningCount++;
                    }

                    // 2. Kiểm tra số môn F
                    var enrollments = await _enrollmentRepository.GetByStudentAsync(student.StudentId);
                    var failedCourses = enrollments.Count(e => e.Grade == "F");

                    if (failedCourses >= 3)
                    {
                        await SendFailedCoursesWarningAsync(student.StudentId, failedCourses);
                        warningCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing warnings for student {student.StudentCode}");
                }
            }

            _logger.LogInformation($"✅ Academic warning check completed. {warningCount} warnings sent.");
            return warningCount;
        }

        public async Task SendGPAWarningAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null) return;

            try
            {
                // 1. Tạo notification trong database
                var notification = new Notification
                {
                    StudentId = student.StudentId,
                    Title = "⚠️ Cảnh báo GPA thấp",
                    Message = $"GPA hiện tại của bạn là {student.OverallGPA:F2}, thấp hơn mức tối thiểu 2.0. Vui lòng cải thiện kết quả học tập.",
                    Type = "warning",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                await _notificationRepository.AddAsync(notification);

                // 2. Gửi real-time notification qua delegate (SignalR ở MVC layer)
                if (OnSendRealTimeNotification != null)
                {
                    await OnSendRealTimeNotification(student.UserId, new NotificationMessageDto
                    {
                        Title = notification.Title,
                        Message = notification.Message,
                        Type = "warning",
                        Link = "/Enrollments/MyEnrollments",
                        CreatedAt = notification.CreatedAt
                    });
                }

                // 3. Gửi email
                if (student.User != null && !string.IsNullOrEmpty(student.User.Email))
                {
                    var emailSubject = "⚠️ Cảnh báo GPA thấp - Cần cải thiện ngay!";
                    var emailBody = BuildGPAWarningEmailBody(student);
                    await _emailService.SendEmailAsync(student.User.Email, emailSubject, emailBody);
                }

                _logger.LogInformation($"📧 GPA warning sent to student {student.StudentCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending GPA warning to student {student.StudentCode}");
                throw;
            }
        }

        public async Task SendFailedCoursesWarningAsync(int studentId, int failedCount)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null) return;

            try
            {
                var notification = new Notification
                {
                    StudentId = student.StudentId,
                    Title = "🚨 Cảnh báo: Nhiều môn học không đạt",
                    Message = $"Bạn đã có {failedCount} môn điểm F. Vui lòng tham khảo cố vấn học tập ngay!",
                    Type = "error",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                await _notificationRepository.AddAsync(notification);

                // Gửi real-time notification
                if (OnSendRealTimeNotification != null)
                {
                    await OnSendRealTimeNotification(student.UserId, new NotificationMessageDto
                    {
                        Title = notification.Title,
                        Message = notification.Message,
                        Type = "error",
                        Link = "/Enrollments/MyEnrollments",
                        CreatedAt = notification.CreatedAt
                    });
                }

                // Gửi email
                if (student.User != null && !string.IsNullOrEmpty(student.User.Email))
                {
                    var emailSubject = $"🚨 Cảnh báo: {failedCount} môn điểm F - Cần hành động ngay!";
                    var emailBody = BuildFailedCoursesEmailBody(student, failedCount);
                    await _emailService.SendEmailAsync(student.User.Email, emailSubject, emailBody);
                }

                _logger.LogInformation($"📧 Failed courses warning sent to student {student.StudentCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending failed courses warning to student {student.StudentCode}");
                throw;
            }
        }

        private string BuildGPAWarningEmailBody(Student student)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background: #ff9800; padding: 30px; text-align: center;'>
                        <h1 style='color: white; margin: 0;'>⚠️ Cảnh báo học vụ</h1>
                    </div>
                    <div style='padding: 30px; background: #fff3cd;'>
                        <h2 style='color: #856404;'>Xin chào {student.FullName}!</h2>
                        <p style='font-size: 16px; color: #856404;'>
                            GPA hiện tại của bạn là <strong style='color: #f44336;'>{student.OverallGPA:F2}</strong>, 
                            thấp hơn mức tối thiểu <strong>2.0</strong>.
                        </p>
                        
                        <div style='background: white; padding: 20px; border-radius: 10px; margin: 20px 0; border-left: 4px solid #ff9800;'>
                            <h4 style='color: #856404; margin-top: 0;'>📋 Khuyến nghị:</h4>
                            <ul style='color: #856404;'>
                                <li>Tập trung ôn tập các môn yếu</li>
                                <li>Tham gia thêm các buổi học phụ đạo</li>
                                <li>Gặp cố vấn học tập để được hỗ trợ</li>
                                <li>Nâng cao điểm số ở các môn sắp tới</li>
                            </ul>
                        </div>

                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='http://localhost:5005/Enrollments/MyEnrollments' 
                               style='background: #ff9800; 
                                      color: white; 
                                      padding: 15px 40px; 
                                      text-decoration: none; 
                                      border-radius: 25px; 
                                      display: inline-block;
                                      font-weight: bold;'>
                                📊 Xem bảng điểm
                            </a>
                        </div>
                    </div>
                </div>
            ";
        }

        private string BuildFailedCoursesEmailBody(Student student, int failedCount)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background: #f44336; padding: 30px; text-align: center;'>
                        <h1 style='color: white; margin: 0;'>🚨 Cảnh báo nghiêm trọng</h1>
                    </div>
                    <div style='padding: 30px; background: #ffebee;'>
                        <h2 style='color: #c62828;'>Xin chào {student.FullName}!</h2>
                        <p style='font-size: 16px; color: #c62828;'>
                            Bạn đã có <strong style='font-size: 24px;'>{failedCount} môn điểm F</strong>. 
                            Điều này có thể ảnh hưởng nghiêm trọng đến quá trình học tập của bạn.
                        </p>
                        
                        <div style='background: white; padding: 20px; border-radius: 10px; margin: 20px 0; border-left: 4px solid #f44336;'>
                            <h4 style='color: #c62828; margin-top: 0;'>⚡ Hành động cần làm ngay:</h4>
                            <ol style='color: #c62828;'>
                                <li><strong>Gặp cố vấn học tập</strong> để được tư vấn</li>
                                <li><strong>Xem xét lại lộ trình học tập</strong></li>
                                <li><strong>Đăng ký học lại</strong> các môn đã trượt</li>
                                <li><strong>Tham gia nhóm học</strong> để cải thiện</li>
                            </ol>
                        </div>

                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='http://localhost:5005/Enrollments/MyEnrollments' 
                               style='background: #f44336; 
                                      color: white; 
                                      padding: 15px 40px; 
                                      text-decoration: none; 
                                      border-radius: 25px; 
                                      display: inline-block;
                                      font-weight: bold;'>
                                📋 Xem chi tiết
                            </a>
                        </div>
                    </div>
                </div>
            ";
        }
    }
}
