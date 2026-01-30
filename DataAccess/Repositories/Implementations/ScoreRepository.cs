using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implementations
{
    public class ScoreRepository : IScoreRepository
    {
        private readonly AppDbContext _context;

        public ScoreRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Score>> GetAllAsync()
        {
            // FIX: Sửa cách Include để tránh null reference
            return await _context.Scores
                .Include(s => s.Student)
                .Include(s => s.Course)
                .Include(s => s.Class)
                .ToListAsync();
        }

        public async Task<IEnumerable<Score>> GetByStudentIdAsync(int studentId)
        {
            // FIX: Sửa cách Include để tránh null reference  
            return await _context.Scores
                .Where(s => s.StudentId == studentId)
                .Include(s => s.Course)
                .Include(s => s.Class)
                .ToListAsync();
        }

        public async Task<Score?> GetByIdAsync(int id)
        {
            return await _context.Scores
                .Include(s => s.Student)
                .FirstOrDefaultAsync(s => s.ScoreId == id);
        }

        public async Task<Score?> GetByStudentAndCourseAsync(int studentId, int courseId)
        {
             return await _context.Scores
                .FirstOrDefaultAsync(s => s.StudentId == studentId && s.CourseId == courseId);
        }

        public async Task<Score> AddAsync(Score score)
        {
            _context.Scores.Add(score);
            await _context.SaveChangesAsync();
            return score;
        }

        public async Task<Score> UpdateAsync(Score score)
        {
            _context.Scores.Update(score);
            await _context.SaveChangesAsync();
            return score;
        }

        public async Task DeleteAsync(int id)
        {
            var score = await _context.Scores.FindAsync(id);
            if (score != null)
            {
                _context.Scores.Remove(score);
                await _context.SaveChangesAsync();
            }
        }
    }
}
