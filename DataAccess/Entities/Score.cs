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

        // Navigation properties
        public virtual Student Student { get; set; } = null!;
    }
}
