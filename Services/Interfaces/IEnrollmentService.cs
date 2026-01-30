using DataAccess.Entities;
using Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Enrollment Service
    /// </summary>
    public interface IEnrollmentService
    {
        // Entity-based methods (for internal use)
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(int id);
        Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId);
        Task<IEnumerable<Enrollment>> GetByClassAsync(int classId);
        Task<Enrollment> CreateAsync(Enrollment enrollment);
        Task<Enrollment> UpdateAsync(Enrollment enrollment);
        Task<Enrollment> UpdateGradeAsync(int enrollmentId, double? midterm, double? final);
        Task DeleteAsync(int id);
        Task<bool> IsEnrolledAsync(int studentId, int classId);
        Task<Enrollment> ChangeClassAsync(int enrollmentId, int newClassId);
        
        // DTO-based methods (for Controllers)
        Task<EnrollmentDto> CreateAsync(EnrollmentCreateDto dto);
    }
}
