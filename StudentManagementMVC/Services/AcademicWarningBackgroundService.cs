using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.Models;
using DataAccess.Entities;
using Microsoft.AspNetCore.SignalR;
using StudentManagementMVC.Hubs;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StudentManagementMVC.Services
{
    /// <summary>
    /// Background Service để kiểm tra và gửi cảnh báo học vụ tự động
    /// </summary>
    public class AcademicWarningBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AcademicWarningBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6); // Kiểm tra mỗi 6 giờ

        public AcademicWarningBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<AcademicWarningBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 Academic Warning Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndSendAcademicWarningsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error in Academic Warning Background Service");
                }

                // Đợi interval trước khi check lại
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("🛑 Academic Warning Background Service stopped");
        }

        private async Task CheckAndSendAcademicWarningsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            
            var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();
            var enrollmentService = scope.ServiceProvider.GetRequiredService<IEnrollmentService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

            try
            {
                _logger.LogInformation("⏰ Checking academic warnings...");

                var allStudents = await studentService.GetAllStudentsAsync();
                int warningCount = 0;

                foreach (var student in allStudents)
                {
                    // 1. Kiểm tra GPA < 2.0
                    if (student.OverallGPA > 0 && student.OverallGPA < 2.0)
                    {
                        await SendGPAWarningAsync(student, notificationService, emailService, hubContext);
                        warningCount++;
                    }

                    // 2. Kiểm tra số môn F
                    var enrollments = await enrollmentService.GetByStudentAsync(student.StudentId);
                    var failedCourses = enrollments.Count(e => e.Grade == "F");

                    if (failedCourses >= 3)
                    {
                        await SendFailedCoursesWarningAsync(student, failedCourses, notificationService, emailService, hubContext);
                        warningCount++;
                    }

                    // 3. Kiểm tra GPA giảm mạnh (> 0.5 điểm trong 1 kỳ)
                    // TODO: Implement semester GPA tracking
                }

                _logger.LogInformation($"✅ Academic warning check completed. {warningCount} warnings sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking academic warnings");
            }
        }

        private async Task SendGPAWarningAsync(
            DataAccess.Entities.Student student,
            INotificationService notificationService,
            IEmailService emailService,
            IHubContext<NotificationHub> hubContext)
        {
            try
            {
                // 1. Tạo notification trong database
                var notification = new DataAccess.Entities.Notification
                {
                    StudentId = student.StudentId,
                    Title = "⚠️ Cảnh báo GPA thấp",
                    Message = $"GPA hiện tại của bạn là {student.OverallGPA:F2}, thấp hơn mức tối thiểu 2.0. Vui lòng cải thiện kết quả học tập.",
                    Type = "warning",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                await notificationService.CreateNotificationAsync(notification);

                // 2. Gửi real-time notification qua SignalR
                await hubContext.Clients.User(student.UserId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        title = notification.Title,
                        message = notification.Message,
                        type = "warning",
                        link = "/Enrollments/MyEnrollments",
                        createdAt = notification.CreatedAt
                    });

                // 3. Gửi email
                if (student.User != null && !string.IsNullOrEmpty(student.User.Email))
                {
                    var emailSubject = "⚠️ Cảnh báo GPA thấp - Cần cải thiện ngay!";
                    var emailBody = $@"
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

                    await emailService.SendEmailAsync(student.User.Email, emailSubject, emailBody);
                }

                _logger.LogInformation($"📧 GPA warning sent to student {student.StudentCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending GPA warning to student {student.StudentCode}");
            }
        }

        private async Task SendFailedCoursesWarningAsync(
            DataAccess.Entities.Student student,
            int failedCount,
            INotificationService notificationService,
            IEmailService emailService,
            IHubContext<NotificationHub> hubContext)
        {
            try
            {
                var notification = new DataAccess.Entities.Notification
                {
                    StudentId = student.StudentId,
                    Title = "🚨 Cảnh báo: Nhiều môn học không đạt",
                    Message = $"Bạn đã có {failedCount} môn điểm F. Vui lòng tham khảo cố vấn học tập ngay!",
                    Type = "error",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                await notificationService.CreateNotificationAsync(notification);

                await hubContext.Clients.User(student.UserId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        title = notification.Title,
                        message = notification.Message,
                        type = "error",
                        link = "/Enrollments/MyEnrollments",
                        createdAt = notification.CreatedAt
                    });

                if (student.User != null && !string.IsNullOrEmpty(student.User.Email))
                {
                    var emailSubject = $"🚨 Cảnh báo: {failedCount} môn điểm F - Cần hành động ngay!";
                    var emailBody = $@"
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

                    await emailService.SendEmailAsync(student.User.Email, emailSubject, emailBody);
                }

                _logger.LogInformation($"📧 Failed courses warning sent to student {student.StudentCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending failed courses warning to student {student.StudentCode}");
            }
        }
    }
}
