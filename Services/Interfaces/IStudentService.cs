using Services.Models;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Student Service
    /// </summary>
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto>> GetAllAsync();
        Task<StudentDto?> GetByIdAsync(string studentId);
        Task<StudentDto?> GetByCodeAsync(string studentCode);
        Task<IEnumerable<StudentDto>> GetByMajorAsync(int majorId);
        Task<IEnumerable<StudentDto>> GetByClassIdAsync(int classId);
        Task<StudentDto?> GetByEmailAsync(string email);
        Task<StudentDto> CreateAsync(StudentCreateDto createDto);
        Task<StudentDto> UpdateAsync(StudentUpdateDto updateDto);
        Task<bool> DeleteAsync(string studentId);
        
        // New methods
        Task<string> GenerateStudentCodeAsync();
        string GenerateDefaultPassword(DateTime dateOfBirth);
        Task<double> CalculateOverallGPAAsync(string studentId);
        Task<StudentDto> CreateStudentWithUserAsync(StudentCreateDto createDto);
        Task<IEnumerable<StudentDto>> SearchStudentsAsync(string searchTerm);
        Task<StudentDto?> UpdateStudentStatusAsync(string studentId, string status);
    }
}
