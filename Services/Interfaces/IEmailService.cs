using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Email Service
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Gửi email chào mừng sinh viên mới
        /// </summary>
        Task SendWelcomeEmailAsync(string toEmail, string studentName, string studentCode, string tempPassword);

        /// <summary>
        /// Gửi email xác nhận đăng ký môn thành công
        /// </summary>
        Task SendEnrollmentConfirmationAsync(string toEmail, string studentName, string courseName, string className);

        /// <summary>
        /// Gửi email thông báo điểm mới
        /// </summary>
        Task SendScoreNotificationAsync(string toEmail, string studentName, string courseName, double gpa, string grade);

        /// <summary>
        /// Gửi email phân tích AI
        /// </summary>
        Task SendAIAnalysisNotificationAsync(string toEmail, string studentName);

        /// <summary>
        /// Gửi email cảnh báo học vụ
        /// </summary>
        Task SendAcademicWarningAsync(string toEmail, string studentName, double overallGPA, string reason);

        /// <summary>
        /// Gửi email gợi ý lộ trình học tập
        /// </summary>
        Task SendLearningPathRecommendationAsync(string toEmail, string studentName);

        /// <summary>
        /// Gửi email tùy chỉnh (generic)
        /// </summary>
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
