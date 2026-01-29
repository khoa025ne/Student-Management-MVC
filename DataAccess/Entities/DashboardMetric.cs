using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity lưu trữ các metrics cho Dashboard
    /// Được cập nhật định kỳ để hiển thị nhanh
    /// </summary>
    public class DashboardMetric
    {
        [Key]
        public int MetricId { get; set; }

        /// <summary>
        /// Tên metric: 
        /// - TOTAL_STUDENTS, TOTAL_TEACHERS, TOTAL_COURSES
        /// - ACTIVE_ENROLLMENTS, AVERAGE_GPA
        /// - PASS_RATE, FAIL_RATE
        /// - LOW_GPA_COUNT, HIGH_GPA_COUNT
        /// - ENROLLMENTS_THIS_SEMESTER
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string MetricName { get; set; } = string.Empty;

        /// <summary>
        /// Giá trị hiện tại
        /// </summary>
        public double CurrentValue { get; set; }

        /// <summary>
        /// Giá trị kỳ trước (để so sánh)
        /// </summary>
        public double? PreviousValue { get; set; }

        /// <summary>
        /// Phần trăm thay đổi
        /// </summary>
        public double? ChangePercentage { get; set; }

        /// <summary>
        /// Xu hướng: Up, Down, Stable
        /// </summary>
        [MaxLength(20)]
        public string? Trend { get; set; }

        /// <summary>
        /// Đơn vị: count, percent, gpa
        /// </summary>
        [MaxLength(20)]
        public string Unit { get; set; } = "count";

        /// <summary>
        /// Category để nhóm các metrics
        /// </summary>
        [MaxLength(50)]
        public string Category { get; set; } = "General";

        /// <summary>
        /// Icon class (FontAwesome)
        /// </summary>
        [MaxLength(50)]
        public string? IconClass { get; set; }

        /// <summary>
        /// Màu sắc hiển thị
        /// </summary>
        [MaxLength(20)]
        public string? ColorClass { get; set; }

        /// <summary>
        /// Semester ID liên quan (nếu có)
        /// </summary>
        public int? SemesterId { get; set; }

        /// <summary>
        /// Ngày cập nhật cuối
        /// </summary>
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// Thứ tự hiển thị
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// Có hiển thị trên dashboard không
        /// </summary>
        public bool IsVisible { get; set; } = true;
    }
}
