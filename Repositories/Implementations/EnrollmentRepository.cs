using DataAccess;
using DataAccess.DAO;
using DataAccess.Entities;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    /// <summary>
    /// Repository cho Enrollment - gọi xuống DAO
    /// </summary>
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly EnrollmentDAO _enrollmentDAO;

        public EnrollmentRepository(AppDbContext context)
        {
            _enrollmentDAO = new EnrollmentDAO(context);
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            return await _enrollmentDAO.GetAllAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _enrollmentDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId)
        {
            return await _enrollmentDAO.GetByStudentAsync(studentId);
        }

        public async Task<IEnumerable<Enrollment>> GetByClassAsync(int classId)
        {
            return await _enrollmentDAO.GetByClassAsync(classId);
        }

        public async Task<Enrollment> AddAsync(Enrollment enrollment)
        {
            return await _enrollmentDAO.AddAsync(enrollment);
        }

        public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
        {
            return await _enrollmentDAO.UpdateAsync(enrollment);
        }

        public async Task DeleteAsync(int id)
        {
            await _enrollmentDAO.DeleteAsync(id);
        }

        public async Task<bool> IsEnrolledAsync(int studentId, int classId)
        {
            return await _enrollmentDAO.IsEnrolledAsync(studentId, classId);
        }
    }
}
