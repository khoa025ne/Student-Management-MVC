using DataAccess;
using DataAccess.DAO;
using DataAccess.Entities;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    /// <summary>
    /// Repository cho Class - gọi xuống DAO
    /// </summary>
    public class ClassRepository : IClassRepository
    {
        private readonly ClassDAO _classDAO;

        public ClassRepository(AppDbContext context)
        {
            _classDAO = new ClassDAO(context);
        }

        public async Task<IEnumerable<Class>> GetAllAsync()
        {
            return await _classDAO.GetAllAsync();
        }

        public async Task<Class?> GetByIdAsync(int id)
        {
            return await _classDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Class>> GetBySemesterAsync(int semesterId)
        {
            return await _classDAO.GetBySemesterAsync(semesterId);
        }

        public async Task<IEnumerable<Class>> GetByCourseAsync(int courseId)
        {
            return await _classDAO.GetByCourseAsync(courseId);
        }

        public async Task<Class> AddAsync(Class classEntity)
        {
            return await _classDAO.AddAsync(classEntity);
        }

        public async Task<Class> UpdateAsync(Class classEntity)
        {
            return await _classDAO.UpdateAsync(classEntity);
        }

        public async Task DeleteAsync(int id)
        {
            await _classDAO.DeleteAsync(id);
        }
    }
}
