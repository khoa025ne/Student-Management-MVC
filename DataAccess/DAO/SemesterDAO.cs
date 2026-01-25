using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    /// <summary>
    /// DAO cho Semester
    /// </summary>
    public class SemesterDAO
    {
        private readonly AppDbContext _context;

        public SemesterDAO(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Semester>> GetAllAsync()
        {
            try
            {
                return await _context.Semesters.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách học kỳ: {ex.Message}", ex);
            }
        }

        public async Task<Semester?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Semesters
                    .Include(s => s.Classes)
                    .FirstOrDefaultAsync(s => s.SemesterId == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy học kỳ theo ID: {ex.Message}", ex);
            }
        }

        public async Task<Semester?> GetActiveAsync()
        {
            try
            {
                return await _context.Semesters
                    .FirstOrDefaultAsync(s => s.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy học kỳ hiện tại: {ex.Message}", ex);
            }
        }

        public async Task<Semester> AddAsync(Semester semester)
        {
            try
            {
                await _context.Semesters.AddAsync(semester);
                await _context.SaveChangesAsync();
                return semester;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm học kỳ: {ex.Message}", ex);
            }
        }

        public async Task<Semester> UpdateAsync(Semester semester)
        {
            try
            {
                _context.Semesters.Update(semester);
                await _context.SaveChangesAsync();
                return semester;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật học kỳ: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var semester = await _context.Semesters.FindAsync(id);
                if (semester != null)
                {
                    _context.Semesters.Remove(semester);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa học kỳ: {ex.Message}", ex);
            }
        }
    }
}
