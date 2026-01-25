using DataAccess;
using DataAccess.DAO;
using DataAccess.Entities;
using DataAccess.Enums;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    /// <summary>
    /// Repository cho Student - gọi xuống DAO
    /// </summary>
    public class StudentRepository : IStudentRepository
    {
        private readonly StudentDAO _studentDAO;

        public StudentRepository(AppDbContext context)
        {
            _studentDAO = new StudentDAO(context);
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _studentDAO.GetAllAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _studentDAO.GetByIdAsync(id);
        }

        public async Task<Student?> GetByCodeAsync(string studentCode)
        {
            return await _studentDAO.GetByCodeAsync(studentCode);
        }

        public async Task<IEnumerable<Student>> GetByMajorAsync(MajorType major)
        {
            return await _studentDAO.GetByMajorAsync(major);
        }

        public async Task<IEnumerable<Student>> GetByTermAsync(int termNo)
        {
            return await _studentDAO.GetByTermAsync(termNo);
        }

        public async Task<IEnumerable<Student>> GetByClassCodeAsync(string classCode)
        {
            return await _studentDAO.GetByClassCodeAsync(classCode);
        }

        public async Task<Student?> GetByUserIdAsync(int userId)
        {
            return await _studentDAO.GetByUserIdAsync(userId);
        }

        public async Task<Student> AddAsync(Student student)
        {
            return await _studentDAO.AddAsync(student);
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            return await _studentDAO.UpdateAsync(student);
        }

        public async Task DeleteAsync(int id)
        {
            await _studentDAO.DeleteAsync(id);
        }
    }
}
