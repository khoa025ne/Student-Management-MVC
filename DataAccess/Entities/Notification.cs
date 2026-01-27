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

        // Liên kết tới Student
        public int? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // Kiểu thông báo + trạng thái đọc
        public string Type { get; set; } = string.Empty;  // "Achievement", "Performance Alert", "Learning Path", "Score Update"
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Link đến trang liên quan (ví dụ: /Students/MyGrades)
        /// </summary>
        [MaxLength(200)]
        public string? Link { get; set; }
    }
}
