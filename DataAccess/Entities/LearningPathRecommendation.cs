using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity lưu trữ gợi ý lộ trình học tập từ AI
    /// </summary>
    public class LearningPathRecommendation
    {
        [Key]
        public int RecommendationId { get; set; }

        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;

        public int SemesterId { get; set; }
        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; } = null!;

        public DateTime RecommendationDate { get; set; } = DateTime.Now;

        // JSON chứa danh sách môn được gợi ý
        public string? RecommendedCoursesJson { get; set; }

        // Chiến lược học tập tổng quát
        public string? OverallStrategy { get; set; }

        // Cảnh báo nếu có
        public string? WarningsJson { get; set; }

        public string AiModelUsed { get; set; } = "Gemini";

        // Đánh dấu sinh viên đã xem chưa
        public bool IsViewed { get; set; } = false;
    }
}
