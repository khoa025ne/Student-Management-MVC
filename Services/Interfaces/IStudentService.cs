using DataAccess.Entities;
using DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Student Service
    /// </summary>
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByCodeAsync(string studentCode);
        Task<IEnumerable<Student>> GetByMajorAsync(MajorType major);
        Task<IEnumerable<Student>> GetByTermAsync(int termNo);
        Task<IEnumerable<Student>> GetByClassCodeAsync(string classCode);
        Task<Student?> GetByUserIdAsync(int userId);
        Task<Student> CreateAsync(Student student);
        Task<Student> UpdateAsync(Student student);
        Task DeleteAsync(int id);
        
        // New methods
        Task<string> GenerateStudentCodeAsync();
        string GenerateDefaultPassword(DateTime dateOfBirth);
        Task<double> CalculateOverallGPAAsync(int studentId);
        Task<Student> CreateStudentWithUserAsync(Student student, string fullName, string email, string? phoneNumber);
    }
}
