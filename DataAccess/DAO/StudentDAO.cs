using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    /// <summary>
    /// DAO cho Student
    /// </summary>
    public class StudentDAO
    {
        private readonly AppDbContext _context;

        public StudentDAO(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            try
            {
                return await _context.Students
                    .Include(s => s.User)
                    .Include(s => s.Enrollments)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách sinh viên: {ex.Message}", ex);
            }
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Students
                    .Include(s => s.User)
                    .Include(s => s.Enrollments)
                    .FirstOrDefaultAsync(s => s.StudentId == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sinh viên theo ID: {ex.Message}", ex);
            }
        }

        public async Task<Student?> GetByCodeAsync(string studentCode)
        {
            try
            {
                return await _context.Students
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.StudentCode == studentCode);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sinh viên theo mã: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Student>> GetByMajorAsync(MajorType major)
        {
            try
            {
                return await _context.Students
                    .Where(s => s.Major == major)
                    .Include(s => s.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sinh viên theo ngành: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Student>> GetByTermAsync(int termNo)
        {
            try
            {
                return await _context.Students
                    .Where(s => s.CurrentTermNo == termNo)
                    .Include(s => s.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sinh viên theo kỳ: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Student>> GetByClassCodeAsync(string classCode)
        {
            try
            {
                return await _context.Students
                    .Where(s => s.ClassCode == classCode)
                    .Include(s => s.User)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sinh viên theo mã lớp: {ex.Message}", ex);
            }
        }

        public async Task<Student> AddAsync(Student student)
        {
            try
            {
                await _context.Students.AddAsync(student);
                await _context.SaveChangesAsync();
                return student;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm sinh viên: {ex.Message}", ex);
            }
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            try
            {
                _context.Students.Update(student);
                await _context.SaveChangesAsync();
                return student;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật sinh viên: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student != null)
                {
                    _context.Students.Remove(student);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa sinh viên: {ex.Message}", ex);
            }
        }

        public async Task<Student?> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Students
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.UserId == userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sinh viên theo UserId: {ex.Message}", ex);
            }
        }
    }
}
