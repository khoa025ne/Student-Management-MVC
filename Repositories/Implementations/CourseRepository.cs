using DataAccess;
using DataAccess.DAO;
using DataAccess.Entities;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    /// <summary>
    /// Repository cho Course - gọi xuống DAO
    /// </summary>
    public class CourseRepository : ICourseRepository
    {
        private readonly CourseDAO _courseDAO;

        public CourseRepository(AppDbContext context)
        {
            _courseDAO = new CourseDAO(context);
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _courseDAO.GetAllAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _courseDAO.GetByIdAsync(id);
        }

        public async Task<Course?> GetByCodeAsync(string courseCode)
        {
            return await _courseDAO.GetByCodeAsync(courseCode);
        }

        public async Task<Course> AddAsync(Course course)
        {
            return await _courseDAO.AddAsync(course);
        }

        public async Task<Course> UpdateAsync(Course course)
        {
            return await _courseDAO.UpdateAsync(course);
        }

        public async Task DeleteAsync(int id)
        {
            await _courseDAO.DeleteAsync(id);
        }
    }
}
