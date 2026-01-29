using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity lưu trữ log các cuộc hội thoại với AI
    /// Để tracking và cải thiện chất lượng response
    /// </summary>
    public class AIConversationLog
    {
        [Key]
        public int LogId { get; set; }

        /// <summary>
        /// ID của sinh viên (nếu có)
        /// </summary>
        public int? StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        /// <summary>
        /// ID của User thực hiện request
        /// </summary>
        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        /// <summary>
        /// Loại request: 
        /// - ACADEMIC_ANALYSIS: Phân tích học tập
        /// - LEARNING_PATH: Gợi ý lộ trình
        /// - COURSE_RECOMMENDATION: Gợi ý môn học
        /// - GENERAL_QUERY: Câu hỏi chung
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string RequestType { get; set; } = string.Empty;

        /// <summary>
        /// Prompt gửi đến AI
        /// </summary>
        [Required]
        public string Prompt { get; set; } = string.Empty;

        /// <summary>
        /// Response từ AI
        /// </summary>
        public string? Response { get; set; }

        /// <summary>
        /// Danh sách KnowledgeBase IDs đã sử dụng (comma-separated)
        /// </summary>
        public string? UsedKnowledgeIds { get; set; }

        /// <summary>
        /// Model AI đã sử dụng: GPT-4, GPT-3.5, Fallback
        /// </summary>
        [MaxLength(50)]
        public string ModelUsed { get; set; } = "GPT-4";

        /// <summary>
        /// Token đã sử dụng
        /// </summary>
        public int TokensUsed { get; set; } = 0;

        /// <summary>
        /// Thời gian xử lý (ms)
        /// </summary>
        public int ProcessingTimeMs { get; set; } = 0;

        /// <summary>
        /// Trạng thái: Success, Failed, Timeout
        /// </summary>
        [MaxLength(20)]
        public string Status { get; set; } = "Success";

        /// <summary>
        /// Error message nếu có
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Rating từ user (1-5)
        /// </summary>
        public int? UserRating { get; set; }

        /// <summary>
        /// Feedback từ user
        /// </summary>
        public string? UserFeedback { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
