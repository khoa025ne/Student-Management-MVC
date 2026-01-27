namespace Services.Models
{
    /// <summary>
    /// Kết quả gợi ý lộ trình học tập từ AI
    /// </summary>
    public class LearningPathResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public RecommendedCourse[]? RecommendedCourses { get; set; }
        public string? OverallStrategy { get; set; }
        public string[]? Warnings { get; set; }
    }

    /// <summary>
    /// Thông tin môn học được gợi ý
    /// </summary>
    public class RecommendedCourse
    {
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        
        /// <summary>
        /// Độ ưu tiên (1 = cao nhất, 4 = thấp nhất)
        /// </summary>
        public int Priority { get; set; }
        
        /// <summary>
        /// Lý do cụ thể tại sao nên học môn này
        /// </summary>
        public string Reason { get; set; } = string.Empty;
        
        /// <summary>
        /// Lợi ích khi học môn này
        /// </summary>
        public string Benefits { get; set; } = string.Empty;
    }
}
