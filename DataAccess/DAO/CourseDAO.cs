using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    /// <summary>
    /// DAO cho Course
    /// </summary>
    public class CourseDAO
    {
        private readonly AppDbContext _context;

        public CourseDAO(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.PrerequisiteCourse)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách môn học: {ex.Message}", ex);
            }
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.PrerequisiteCourse)
                    .FirstOrDefaultAsync(c => c.CourseId == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy môn học theo ID: {ex.Message}", ex);
            }
        }

        public async Task<Course?> GetByCodeAsync(string courseCode)
        {
            try
            {
                return await _context.Courses
                    .Include(c => c.PrerequisiteCourse)
                    .FirstOrDefaultAsync(c => c.CourseCode == courseCode);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy môn học theo mã: {ex.Message}", ex);
            }
        }

        public async Task<Course> AddAsync(Course course)
        {
            try
            {
                await _context.Courses.AddAsync(course);
                await _context.SaveChangesAsync();
                return course;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm môn học: {ex.Message}", ex);
            }
        }

        public async Task<Course> UpdateAsync(Course course)
        {
            try
            {
                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
                return course;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật môn học: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var course = await _context.Courses.FindAsync(id);
                if (course != null)
                {
                    _context.Courses.Remove(course);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa môn học: {ex.Message}", ex);
            }
        }
    }
}
