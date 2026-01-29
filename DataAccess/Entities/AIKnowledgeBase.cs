using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity lưu trữ Knowledge Base cho AI
    /// Chứa các quy tắc, hướng dẫn, và nội dung để AI tham khảo khi generate response
    /// Phù hợp với giáo trình FPT University
    /// </summary>
    public class AIKnowledgeBase
    {
        [Key]
        public int KnowledgeId { get; set; }

        /// <summary>
        /// Tiêu đề/tên của knowledge item
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Nội dung chi tiết của knowledge item
        /// Đây là phần AI sẽ đọc để tham khảo
        /// </summary>
        [Required]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Danh mục phân loại: 
        /// - FLOW1_REGISTRATION: Đăng ký sinh viên
        /// - FLOW2_GRADING: Nhập điểm
        /// - FLOW3_NOTIFICATION: Thông báo
        /// - COURSE_PREREQUISITE: Điều kiện tiên quyết
        /// - GPA_CALCULATION: Tính toán GPA
        /// - ACADEMIC_WARNING: Cảnh báo học vụ
        /// - LEARNING_PATH: Lộ trình học tập
        /// - FPT_CURRICULUM: Giáo trình FPT
        /// - VALIDATION_RULE: Quy tắc validation
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        /// <summary>
        /// Subcategory để phân loại chi tiết hơn
        /// </summary>
        [MaxLength(100)]
        public string? SubCategory { get; set; }

        /// <summary>
        /// Tags để tìm kiếm nhanh (comma-separated)
        /// VD: "gpa,score,midterm,final"
        /// </summary>
        [MaxLength(500)]
        public string? Tags { get; set; }

        /// <summary>
        /// Độ ưu tiên khi AI lựa chọn (1-10, 10 là cao nhất)
        /// </summary>
        public int Priority { get; set; } = 5;

        /// <summary>
        /// Số lần được AI sử dụng
        /// </summary>
        public int UsageCount { get; set; } = 0;

        /// <summary>
        /// Trạng thái hoạt động
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Ngôn ngữ của nội dung: vi, en
        /// </summary>
        [MaxLength(10)]
        public string Language { get; set; } = "vi";

        /// <summary>
        /// Metadata bổ sung dạng JSON
        /// </summary>
        public string? MetadataJson { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Ngày cập nhật cuối
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Người tạo
        /// </summary>
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
    }
}
