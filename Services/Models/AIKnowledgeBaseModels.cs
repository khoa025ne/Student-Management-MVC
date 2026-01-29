using System;
using System.Collections.Generic;

namespace Services.Models
{
    /// <summary>
    /// Model cho AI Knowledge Base
    /// </summary>
    public class AIKnowledgeBaseModel
    {
        public int KnowledgeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? SubCategory { get; set; }
        public string? Tags { get; set; }
        public int Priority { get; set; } = 5;
        public int UsageCount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public string Language { get; set; } = "vi";
        public string? MetadataJson { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }

    /// <summary>
    /// Request để AI xử lý với Knowledge Base
    /// </summary>
    public class AIProcessingRequest
    {
        public int? StudentId { get; set; }
        public string RequestType { get; set; } = string.Empty; // ACADEMIC_ANALYSIS, LEARNING_PATH, COURSE_RECOMMENDATION
        public string UserQuery { get; set; } = string.Empty;
        public Dictionary<string, object>? AdditionalData { get; set; }
    }

    /// <summary>
    /// Response từ AI sau khi xử lý
    /// </summary>
    public class AIProcessingResponse
    {
        public bool Success { get; set; }
        public string? Response { get; set; }
        public string? ErrorMessage { get; set; }
        public List<int>? UsedKnowledgeIds { get; set; }
        public string ModelUsed { get; set; } = "GPT-4";
        public int TokensUsed { get; set; }
        public int ProcessingTimeMs { get; set; }
    }

    /// <summary>
    /// Model cho việc phân tích học tập (Flow 2)
    /// </summary>
    public class AcademicAnalysisRequest
    {
        public int StudentId { get; set; }
        public double OverallGPA { get; set; }
        public List<CompletedCourseInfo> CompletedCourses { get; set; } = new();
    }

    public class CompletedCourseInfo
    {
        public string CourseName { get; set; } = string.Empty;
        public double GPA { get; set; }
        public string Grade { get; set; } = string.Empty;
        public int Credits { get; set; }
    }

    /// <summary>
    /// Response từ AI cho phân tích học tập
    /// </summary>
    public class AcademicAnalysisResponse
    {
        public List<string> StrongSubjects { get; set; } = new();
        public List<string> WeakSubjects { get; set; } = new();
        public string Recommendations { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model cho Learning Path Recommendation (Flow 3)
    /// </summary>
    public class LearningPathRequest
    {
        public int StudentId { get; set; }
        public double CurrentGPA { get; set; }
        public List<string> StrongSubjects { get; set; } = new();
        public List<string> WeakSubjects { get; set; } = new();
        public List<string> AvailableCourses { get; set; } = new();
    }

    /// <summary>
    /// Response từ AI cho Learning Path
    /// </summary>
    public class LearningPathResponse
    {
        public List<RecommendedCourse> RecommendedCourses { get; set; } = new();
        public string OverallStrategy { get; set; } = string.Empty;
    }

    // Note: RecommendedCourse is defined in LearningPathResult.cs

    /// <summary>
    /// Categories cho AI Knowledge Base
    /// </summary>
    public static class AIKnowledgeCategories
    {
        public const string FLOW1_REGISTRATION = "FLOW1_REGISTRATION";
        public const string FLOW2_GRADING = "FLOW2_GRADING";
        public const string FLOW3_NOTIFICATION = "FLOW3_NOTIFICATION";
        public const string COURSE_PREREQUISITE = "COURSE_PREREQUISITE";
        public const string GPA_CALCULATION = "GPA_CALCULATION";
        public const string ACADEMIC_WARNING = "ACADEMIC_WARNING";
        public const string LEARNING_PATH = "LEARNING_PATH";
        public const string FPT_CURRICULUM = "FPT_CURRICULUM";
        public const string VALIDATION_RULE = "VALIDATION_RULE";
        public const string GENERAL_GUIDANCE = "GENERAL_GUIDANCE";

        public static Dictionary<string, string> GetAllCategories()
        {
            return new Dictionary<string, string>
            {
                { FLOW1_REGISTRATION, "Flow 1: Đăng ký sinh viên" },
                { FLOW2_GRADING, "Flow 2: Nhập điểm" },
                { FLOW3_NOTIFICATION, "Flow 3: Thông báo & Lộ trình" },
                { COURSE_PREREQUISITE, "Điều kiện tiên quyết" },
                { GPA_CALCULATION, "Tính toán GPA" },
                { ACADEMIC_WARNING, "Cảnh báo học vụ" },
                { LEARNING_PATH, "Lộ trình học tập" },
                { FPT_CURRICULUM, "Giáo trình FPT" },
                { VALIDATION_RULE, "Quy tắc validation" },
                { GENERAL_GUIDANCE, "Hướng dẫn chung" }
            };
        }
    }
}
