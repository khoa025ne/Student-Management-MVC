using System;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity đại diện cho giảng viên trong hệ thống
    /// </summary>
    public class Teacher
    {
        public int Id { get; set; }
        public string TeacherCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string? Degree { get; set; } // ThS, TS, PGS.TS, GS.TS
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Foreign Key to User
        public int? UserId { get; set; }
        public virtual User? User { get; set; }

        // Navigation Properties
        public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
    }
}
