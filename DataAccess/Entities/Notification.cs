using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity đại diện cho thông báo
    /// </summary>
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        // Alias for easier access
        [NotMapped]
        public int Id => NotificationId;

        // Liên kết tới Student
        public int? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        // Liên kết tới Teacher (cho Flow 2)
        public int? TeacherId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // Kiểu thông báo + trạng thái đọc
        public string Type { get; set; } = string.Empty;  // "Achievement", "Performance Alert", "Learning Path", "Score Update"
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // TailAdmin - Extended Properties
        public DateTime? ReadAt { get; set; }
        public string? CreatedBy { get; set; }
        
        /// <summary>
        /// Priority: "high", "medium", "low"
        /// </summary>
        [MaxLength(20)]
        public string Priority { get; set; } = "medium";
        
        /// <summary>
        /// Link đến trang liên quan (ví dụ: /Students/MyGrades)
        /// </summary>
        [MaxLength(200)]
        public string? Link { get; set; }

        /// <summary>
        /// Alias for Link (ActionUrl for TailAdmin compatibility)
        /// </summary>
        [NotMapped]
        public string? ActionUrl => Link;
    }
}
