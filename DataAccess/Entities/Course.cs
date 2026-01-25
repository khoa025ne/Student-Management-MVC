using System;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity đại diện cho môn học
    /// </summary>
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty; // Lập trình C#
        public string CourseCode { get; set; } = string.Empty; // PRN211
        public int Credits { get; set; } // Số tín chỉ: 3
        public string? Major { get; set; } // Chuyên ngành: SE, AI, IS, etc.

        // Tự tham chiếu để làm điều kiện tiên quyết (Prerequisite)
        public int? PrerequisiteCourseId { get; set; }
        public virtual Course? PrerequisiteCourse { get; set; }

        // Navigation
        public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
