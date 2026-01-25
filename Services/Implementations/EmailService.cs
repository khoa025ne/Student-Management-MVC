using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Services.Interfaces;
using Services.Models;
using System;
using System.Threading.Tasks;

namespace Services.Implementations
{
    /// <summary>
    /// Email Service sử dụng MailKit để gửi email
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string studentName, string studentCode, string tempPassword)
        {
            var subject = "Chào mừng bạn đến với Hệ thống Quản lý Sinh viên";
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .info-box {{ background-color: white; padding: 15px; margin: 10px 0; border-left: 4px solid #007bff; }}
                        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
                        .button {{ display: inline-block; padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Chào mừng đến với Hệ thống!</h1>
                        </div>
                        <div class='content'>
                            <p>Xin chào <strong>{studentName}</strong>,</p>
                            <p>Chúc mừng bạn đã được tạo tài khoản trong Hệ thống Quản lý Sinh viên. Dưới đây là thông tin đăng nhập của bạn:</p>
                            
                            <div class='info-box'>
                                <p><strong>Mã sinh viên:</strong> {studentCode}</p>
                                <p><strong>Email:</strong> {toEmail}</p>
                                <p><strong>Mật khẩu tạm thời:</strong> {tempPassword}</p>
                            </div>
                            
                            <p><strong style='color: red;'>⚠️ LƯU Ý QUAN TRỌNG:</strong></p>
                            <ul>
                                <li>Vui lòng đổi mật khẩu ngay sau lần đăng nhập đầu tiên</li>
                                <li>Mật khẩu mới phải có tối thiểu 8 ký tự, bao gồm chữ HOA, chữ thường, số và ký tự đặc biệt</li>
                                <li>Không chia sẻ mật khẩu với bất kỳ ai</li>
                            </ul>
                            
                            <p style='text-align: center; margin: 30px 0;'>
                                <a href='#' class='button'>Đăng nhập ngay</a>
                            </p>
                        </div>
                        <div class='footer'>
                            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
                            <p>&copy; 2026 Student Management System. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendEnrollmentConfirmationAsync(string toEmail, string studentName, string courseName, string className)
        {
            var subject = "Xác nhận đăng ký môn học thành công";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #28a745;'>✅ Đăng ký thành công!</h2>
                        <p>Xin chào <strong>{studentName}</strong>,</p>
                        <p>Bạn đã đăng ký thành công môn học:</p>
                        <div style='background-color: #f0f0f0; padding: 15px; margin: 20px 0;'>
                            <p><strong>Môn học:</strong> {courseName}</p>
                            <p><strong>Lớp học:</strong> {className}</p>
                            <p><strong>Ngày đăng ký:</strong> {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                        </div>
                        <p>Vui lòng kiểm tra lịch học và chuẩn bị tài liệu cho buổi học đầu tiên.</p>
                        <p>Chúc bạn học tập tốt!</p>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendScoreNotificationAsync(string toEmail, string studentName, string courseName, double gpa, string grade)
        {
            var gradeColor = grade == "F" ? "red" : (grade.StartsWith("A") ? "green" : "orange");
            var subject = $"Thông báo điểm mới - {courseName}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #007bff;'>📊 Điểm mới đã có!</h2>
                        <p>Xin chào <strong>{studentName}</strong>,</p>
                        <p>Điểm của bạn cho môn <strong>{courseName}</strong> đã được cập nhật:</p>
                        <div style='background-color: #f9f9f9; padding: 20px; margin: 20px 0; border-left: 5px solid {gradeColor};'>
                            <h3 style='margin: 0; color: {gradeColor};'>GPA: {gpa:F2}</h3>
                            <h3 style='margin: 10px 0 0 0; color: {gradeColor};'>Grade: {grade}</h3>
                        </div>
                        <p>Hệ thống đã tạo phân tích học tập cho bạn. Vui lòng đăng nhập để xem chi tiết.</p>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendAIAnalysisNotificationAsync(string toEmail, string studentName)
        {
            var subject = "AI đã phân tích kết quả học tập của bạn";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #6f42c1;'>🤖 Phân tích AI mới</h2>
                        <p>Xin chào <strong>{studentName}</strong>,</p>
                        <p>Hệ thống AI đã hoàn tất phân tích kết quả học tập của bạn!</p>
                        <p>Phân tích bao gồm:</p>
                        <ul>
                            <li>Các môn học bạn đang làm tốt</li>
                            <li>Các môn học cần cải thiện</li>
                            <li>Khuyến nghị học tập cụ thể</li>
                        </ul>
                        <p style='text-align: center; margin: 30px 0;'>
                            <a href='#' style='display: inline-block; padding: 12px 30px; background-color: #6f42c1; color: white; text-decoration: none; border-radius: 5px;'>Xem phân tích</a>
                        </p>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendAcademicWarningAsync(string toEmail, string studentName, double overallGPA, string reason)
        {
            var subject = "⚠️ CẢNH BÁO HỌC VỤ";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='background-color: #dc3545; color: white; padding: 20px; text-align: center;'>
                            <h1>⚠️ CẢNH BÁO HỌC VỤ</h1>
                        </div>
                        <div style='padding: 20px;'>
                            <p>Xin chào <strong>{studentName}</strong>,</p>
                            <p style='color: red; font-size: 16px;'><strong>Hệ thống phát hiện vấn đề về kết quả học tập của bạn:</strong></p>
                            <div style='background-color: #fff3cd; border-left: 5px solid #ffc107; padding: 15px; margin: 20px 0;'>
                                <p><strong>GPA tổng kết hiện tại:</strong> {overallGPA:F2}</p>
                                <p><strong>Lý do cảnh báo:</strong> {reason}</p>
                            </div>
                            <p><strong>Khuyến nghị:</strong></p>
                            <ul>
                                <li>Gặp gỡ cố vấn học tập để được hỗ trợ</li>
                                <li>Tham gia các lớp học bổ trợ</li>
                                <li>Xem lại phương pháp học tập</li>
                                <li>Tập trung hơn vào các môn yếu</li>
                            </ul>
                            <p style='color: #dc3545;'><strong>Lưu ý:</strong> Nếu tình trạng không được cải thiện, bạn có thể bị đình chỉ học tập.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendLearningPathRecommendationAsync(string toEmail, string studentName)
        {
            var subject = "🎯 AI đề xuất lộ trình học tập cho bạn";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #17a2b8;'>🎯 Lộ trình học tập được đề xuất</h2>
                        <p>Xin chào <strong>{studentName}</strong>,</p>
                        <p>Dựa trên kết quả học tập và sở thích của bạn, AI đã đề xuất lộ trình học tập phù hợp cho kỳ học tới!</p>
                        <p>Lộ trình bao gồm:</p>
                        <ul>
                            <li>Các môn học được ưu tiên theo thứ tự</li>
                            <li>Lý do tại sao nên học môn đó</li>
                            <li>Chiến lược học tập tổng quát</li>
                        </ul>
                        <p style='text-align: center; margin: 30px 0;'>
                            <a href='#' style='display: inline-block; padding: 12px 30px; background-color: #17a2b8; color: white; text-decoration: none; border-radius: 5px;'>Xem lộ trình</a>
                        </p>
                    </div>
                </body>
                </html>
            ";

            await SendEmailAsync(toEmail, subject, body);
        }

        /// <summary>
        /// Method core để gửi email
        /// </summary>
        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log error (có thể dùng ILogger)
                throw new Exception($"Lỗi khi gửi email: {ex.Message}", ex);
            }
        }
    }
}
