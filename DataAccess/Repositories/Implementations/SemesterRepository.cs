using DataAccess.DAO;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implementations
{
    /// <summary>
    /// Repository cho Semester - gọi xuống DAO
    /// </summary>
    public class SemesterRepository : ISemesterRepository
    {
        private readonly SemesterDAO _semesterDAO;

        public SemesterRepository(AppDbContext context)
        {
            _semesterDAO = new SemesterDAO(context);
        }

        public async Task<IEnumerable<Semester>> GetAllAsync()
        {
            return await _semesterDAO.GetAllAsync();
        }

        public async Task<Semester?> GetByIdAsync(int id)
        {
            return await _semesterDAO.GetByIdAsync(id);
        }

        public async Task<Semester?> GetActiveAsync()
        {
            return await _semesterDAO.GetActiveAsync();
        }

        public async Task<Semester> AddAsync(Semester semester)
        {
            return await _semesterDAO.AddAsync(semester);
        }

        public async Task<Semester> UpdateAsync(Semester semester)
        {
            return await _semesterDAO.UpdateAsync(semester);
        }

        public async Task DeleteAsync(int id)
        {
            await _semesterDAO.DeleteAsync(id);
        }
    }
}
