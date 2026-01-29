using DataAccess.Entities;
using Services.Models;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Student Service
    /// Lưu ý: Trả về Entity cho các operations phức tạp cần navigation properties
    /// </summary>
    public interface IStudentService
    {
        // ===== ENTITY-BASED METHODS (for complex operations) =====
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetByIdAsync(int studentId);
        Task<Student?> GetByUserIdAsync(int userId);
        Task<Student?> GetStudentByCodeAsync(string studentCode);
        Task<Student> CreateAsync(Student student);
        Task<Student> UpdateAsync(Student student);
        Task DeleteAsync(int studentId);
        
        // ===== DTO-BASED METHODS (for simple data transfer) =====
        Task<IEnumerable<StudentDto>> GetAllAsync();
        Task<StudentDto?> GetStudentDtoByIdAsync(int studentId);
        Task<StudentDto?> GetByCodeAsync(string studentCode);
        Task<IEnumerable<StudentDto>> GetByMajorAsync(MajorType major);
        Task<IEnumerable<StudentDto>> GetByClassCodeAsync(string classCode);
        Task<StudentDto?> GetByEmailAsync(string email);
        Task<StudentDto> CreateAsync(StudentCreateDto createDto);
        Task<StudentDto> UpdateAsync(StudentUpdateDto updateDto);
        Task<bool> DeleteStudentAsync(int studentId);
        
        // ===== BUSINESS LOGIC METHODS =====
        Task<string> GenerateStudentCodeAsync();
        string GenerateDefaultPassword(DateTime dateOfBirth);
        Task<double> CalculateOverallGPAAsync(int studentId);
        Task<StudentDto> CreateStudentWithUserAsync(StudentCreateDto createDto);
        Task<IEnumerable<StudentDto>> SearchStudentsAsync(string searchTerm);
        Task<StudentDto?> UpdateStudentStatusAsync(int studentId, string status);
        
        // ===== DASHBOARD =====
        Task<StudentDashboardDto?> GetDashboardAsync(int userId);
        Task<int> GetUnreadNotificationCountAsync(int studentId);
    }
}
