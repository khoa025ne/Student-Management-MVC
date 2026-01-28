using Services.Models;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Class Service
    /// </summary>
    public interface IClassService
    {
        Task<IEnumerable<ClassDto>> GetAllAsync();
        Task<ClassDto?> GetByIdAsync(int classId);
        Task<IEnumerable<ClassDto>> GetBySemesterAsync(int semesterId);
        Task<IEnumerable<ClassDto>> GetByCourseAsync(int courseId);
        Task<ClassDto> CreateAsync(ClassCreateDto createDto);
        Task<ClassDto> UpdateAsync(ClassUpdateDto updateDto);
        Task<bool> DeleteAsync(int classId);
        Task<IEnumerable<ClassDto>> GetActiveClassesAsync();
        Task<ClassDto?> UpdateClassStatusAsync(int classId, bool isActive);
        Task<IEnumerable<ClassDto>> SearchClassesAsync(string searchTerm);
    }
}
