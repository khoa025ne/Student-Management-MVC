using System;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity đại diện cho điểm số của sinh viên theo từng lớp
    /// Hỗ trợ cấu trúc điểm FPT: Chuyên cần (10%) + BT1 (10%) + BT2 (10%) + Giữa kỳ (20%) + Cuối kỳ (50%)
    /// </summary>
    public class Score
    {
        public int ScoreId { get; set; }
        public int StudentId { get; set; }
        public int? ClassId { get; set; }   // Nullable - some scores may not be linked to a class
        public int? CourseId { get; set; }  // Nullable - backward compatibility
        
        // Các cột điểm thành phần
        public double? AttendanceScore { get; set; }    // Điểm chuyên cần (10%)
        public double? Assignment1Score { get; set; }   // Điểm bài tập 1 (10%)
        public double? Assignment2Score { get; set; }   // Điểm bài tập 2 (10%)
        public double? MidtermScore { get; set; }       // Điểm giữa kỳ (20%)
        public double? FinalExamScore { get; set; }     // Điểm cuối kỳ (50%)
        
        // Điểm tổng kết
        public double? TotalScore { get; set; }         // Tổng điểm (tính tự động)
        public string? Grade { get; set; }              // Xếp loại: A, B, C, D, F
        
        // Metadata
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Comment { get; set; }            // Nhận xét của giảng viên
        
        // Backward compatibility
        public double ScoreValue { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; } = null!;
        public virtual Course? Course { get; set; }
        public virtual Class? Class { get; set; }
        
        /// <summary>
        /// Tính điểm tổng kết theo công thức FPT
        /// Chuyên cần (10%) + BT1 (10%) + BT2 (10%) + Giữa kỳ (20%) + Cuối kỳ (50%)
        /// </summary>
        public void CalculateTotalScore()
        {
            double total = 0;
            total += (AttendanceScore ?? 0) * 0.10;
            total += (Assignment1Score ?? 0) * 0.10;
            total += (Assignment2Score ?? 0) * 0.10;
            total += (MidtermScore ?? 0) * 0.20;
            total += (FinalExamScore ?? 0) * 0.50;
            
            TotalScore = Math.Round(total, 2);
            Grade = CalculateGrade(TotalScore.Value);
        }
        
        /// <summary>
        /// Tính xếp loại dựa trên điểm tổng kết
        /// </summary>
        private static string CalculateGrade(double score)
        {
            return score switch
            {
                >= 8.5 => "A",
                >= 7.0 => "B",
                >= 5.5 => "C",
                >= 4.0 => "D",
                _ => "F"
            };
        }
    }
}
