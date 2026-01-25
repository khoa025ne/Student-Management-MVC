using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    /// <summary>
    /// DAO cho Class
    /// </summary>
    public class ClassDAO
    {
        private readonly AppDbContext _context;

        public ClassDAO(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Class>> GetAllAsync()
        {
            try
            {
                return await _context.Classes
                    .AsNoTracking()
                    .Include(c => c.Course)
                    .Include(c => c.Semester)
                    .Select(c => new Class
                    {
                        ClassId = c.ClassId,
                        ClassName = c.ClassName,
                        ClassCode = c.ClassCode,
                        Schedule = c.Schedule,
                        Room = c.Room,
                        MaxCapacity = c.MaxCapacity,
                        CourseId = c.CourseId,
                        SemesterId = c.SemesterId,
                        Course = c.Course,
                        Semester = c.Semester,
                        Enrollments = c.Enrollments.Take(0).ToList() // Không load enrollments ở Index
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách lớp học: {ex.Message}", ex);
            }
        }

        public async Task<Class?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Classes
                    .AsNoTracking()
                    .Include(c => c.Course)
                    .Include(c => c.Semester)
                    .Include(c => c.Enrollments.Take(100))
                        .ThenInclude(e => e.Student)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(c => c.ClassId == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy lớp học theo ID: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Class>> GetBySemesterAsync(int semesterId)
        {
            try
            {
                return await _context.Classes
                    .Where(c => c.SemesterId == semesterId)
                    .Include(c => c.Course)
                    .Include(c => c.Semester)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy lớp học theo học kỳ: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Class>> GetByCourseAsync(int courseId)
        {
            try
            {
                return await _context.Classes
                    .Where(c => c.CourseId == courseId)
                    .Include(c => c.Course)
                    .Include(c => c.Semester)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy lớp học theo môn: {ex.Message}", ex);
            }
        }

        public async Task<Class> AddAsync(Class classEntity)
        {
            try
            {
                await _context.Classes.AddAsync(classEntity);
                await _context.SaveChangesAsync();
                return classEntity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm lớp học: {ex.Message}", ex);
            }
        }

        public async Task<Class> UpdateAsync(Class classEntity)
        {
            try
            {
                _context.Classes.Update(classEntity);
                await _context.SaveChangesAsync();
                return classEntity;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật lớp học: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var classEntity = await _context.Classes.FindAsync(id);
                if (classEntity != null)
                {
                    _context.Classes.Remove(classEntity);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa lớp học: {ex.Message}", ex);
            }
        }
    }
}
