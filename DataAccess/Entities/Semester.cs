using System;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity đại diện cho học kỳ
    /// </summary>
    public class Semester
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty; // Spring 2024
        public string SemesterCode { get; set; } = string.Empty; // SPR24
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } // Kỳ hiện tại đang học

        // Navigation
        public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
