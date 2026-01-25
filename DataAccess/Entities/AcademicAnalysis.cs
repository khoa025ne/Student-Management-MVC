using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity lưu trữ phân tích học tập từ AI
    /// </summary>
    public class AcademicAnalysis
    {
        [Key]
        public int AnalysisId { get; set; }

        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;

        public DateTime AnalysisDate { get; set; } = DateTime.Now;
        public double OverallGPA { get; set; }

        // Lưu JSON string
        public string? StrongSubjectsJson { get; set; } // ["Math", "Physics"]
        public string? WeakSubjectsJson { get; set; }   // ["History"]

        public string? Recommendations { get; set; } // "Nên tập trung..."
        public string AiModelUsed { get; set; } = "GPT-4"; // "GPT-4" or "Fallback"
    }
}
