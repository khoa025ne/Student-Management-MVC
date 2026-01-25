using System;
using System.Collections.Generic;

namespace DataAccess.Entities
{
    /// <summary>
    /// Entity đại diện cho vai trò người dùng trong hệ thống
    /// </summary>
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty; // Admin, Manager, Teacher, Student
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
