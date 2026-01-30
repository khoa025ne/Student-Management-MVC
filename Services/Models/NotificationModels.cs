using System.ComponentModel.DataAnnotations;

namespace Services.Models
{
    /// <summary>
    /// DTO cho NotificationCenter
    /// </summary>
    public class NotificationCenterDto
    {
        public List<NotificationItemDto> Notifications { get; set; } = new();
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? CurrentFilter { get; set; }
        public bool UnreadOnly { get; set; }
    }

    /// <summary>
    /// DTO cho Notification Item trong NotificationCenter
    /// </summary>
    public class NotificationItemDto
    {
        public int Id { get; set; }
        public int NotificationId { get => Id; set => Id = value; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Type { get; set; } = "";
        public string Priority { get; set; } = "low";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? Link { get; set; }
        public string? ActionUrl { get => Link; set => Link = value; }
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
    }

    /// <summary>
    /// DTO cho Notification (full) - dùng trong Controllers
    /// </summary>
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Type { get; set; } = "Info";
        public int Priority { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? ActionUrl { get; set; }
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
        public int? CreatedBy { get; set; }
    }

    /// <summary>
    /// DTO cho tạo Notification
    /// </summary>
    public class CreateNotificationDto
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        public string Title { get; set; } = "";
        
        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string Message { get; set; } = "";
        public string Type { get; set; } = "Info";
        public int Priority { get; set; } = 0;
        public string? ActionUrl { get; set; }
        public bool SendToAll { get; set; }
        public List<int>? ClassIds { get; set; }
        public List<int>? StudentIds { get; set; }
    }
    
    /// <summary>
    /// DTO để update Notification
    /// </summary>
    public class NotificationUpdateDto
    {
        public int NotificationId { get; set; }
        
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        public string Title { get; set; } = "";
        
        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string Message { get; set; } = "";
        public string Type { get; set; } = "Info";
        public int Priority { get; set; }
        public string? ActionUrl { get; set; }
    }

    /// <summary>
    /// DTO cho Class dùng trong Notification
    /// </summary>
    public class NotificationClassDto
    {
        public int ClassId { get; set; }
        public string ClassCode { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string CourseName { get; set; } = "";
    }

    /// <summary>
    /// DTO cho real-time notification message (SignalR)
    /// </summary>
    public class NotificationMessageDto
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Type { get; set; } = "info";
        public string? Link { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
