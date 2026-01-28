using System;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity đại diện cho điểm số của sinh viên
    /// </summary>
    public class Score
    {
        public int ScoreId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public double ScoreValue { get; set; } // Ví dụ điểm số
        public double? TotalScore { get; set; } // Tổng điểm (nếu cần)

        // Navigation properties
        public virtual Student Student { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;
    }
}
