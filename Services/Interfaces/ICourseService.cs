using DataAccess.Entities;
using Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Course Service
    /// </summary>
    public interface ICourseService
    {
        // Entity-based methods (for complex operations)
        Task<IEnumerable<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(int id);
        Task<Course?> GetByCodeAsync(string courseCode);
        Task<Course> CreateAsync(Course course);
        Task<Course> UpdateAsync(Course course);
        Task DeleteAsync(int id);
        
        // DTO-based methods (for Controllers)
        Task<CourseDto> CreateDtoAsync(CourseCreateDto dto);
        Task<CourseDto> UpdateDtoAsync(CourseUpdateDto dto);
    }
}
