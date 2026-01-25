using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    /// <summary>
    /// DAO cho Enrollment
    /// </summary>
    public class EnrollmentDAO
    {
        private readonly AppDbContext _context;

        public EnrollmentDAO(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            try
            {
                return await _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Class)
                        .ThenInclude(c => c.Course)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách đăng ký: {ex.Message}", ex);
            }
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Class)
                        .ThenInclude(c => c.Course)
                    .FirstOrDefaultAsync(e => e.EnrollmentId == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy đăng ký theo ID: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentAsync(int studentId)
        {
            try
            {
                return await _context.Enrollments
                    .Where(e => e.StudentId == studentId)
                    .Include(e => e.Class)
                        .ThenInclude(c => c.Course)
                    .Include(e => e.Class)
                        .ThenInclude(c => c.Semester)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy đăng ký theo sinh viên: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Enrollment>> GetByClassAsync(int classId)
        {
            try
            {
                return await _context.Enrollments
                    .Where(e => e.ClassId == classId)
                    .Include(e => e.Student)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy đăng ký theo lớp: {ex.Message}", ex);
            }
        }

        public async Task<Enrollment> AddAsync(Enrollment enrollment)
        {
            try
            {
                await _context.Enrollments.AddAsync(enrollment);
                await _context.SaveChangesAsync();
                return enrollment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm đăng ký: {ex.Message}", ex);
            }
        }

        public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
        {
            try
            {
                _context.Enrollments.Update(enrollment);
                await _context.SaveChangesAsync();
                return enrollment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật đăng ký: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var enrollment = await _context.Enrollments.FindAsync(id);
                if (enrollment != null)
                {
                    _context.Enrollments.Remove(enrollment);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa đăng ký: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsEnrolledAsync(int studentId, int classId)
        {
            try
            {
                return await _context.Enrollments
                    .AnyAsync(e => e.StudentId == studentId && e.ClassId == classId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra đăng ký: {ex.Message}", ex);
            }
        }
    }
}
