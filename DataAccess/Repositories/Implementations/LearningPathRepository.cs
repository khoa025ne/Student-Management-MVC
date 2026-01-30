using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implementations
{
    public class LearningPathRepository : ILearningPathRepository
    {
        private readonly AppDbContext _context;

        public LearningPathRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LearningPathRecommendation>> GetByStudentIdAsync(int studentId)
        {
            return await _context.LearningPathRecommendations
                .Include(r => r.Semester)
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.RecommendationDate)
                .ToListAsync();
        }

        public async Task<LearningPathRecommendation?> GetLatestByStudentAsync(int studentId)
        {
            return await _context.LearningPathRecommendations
                .Include(r => r.Semester)
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.RecommendationDate)
                .FirstOrDefaultAsync();
        }

        public async Task<LearningPathRecommendation> AddAsync(LearningPathRecommendation recommendation)
        {
            _context.LearningPathRecommendations.Add(recommendation);
            await _context.SaveChangesAsync();
            return recommendation;
        }
    }
}
