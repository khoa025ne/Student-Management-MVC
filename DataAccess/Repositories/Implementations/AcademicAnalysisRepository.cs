using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation cho Academic Analysis
    /// </summary>
    public class AcademicAnalysisRepository : IAcademicAnalysisRepository
    {
        private readonly AppDbContext _context;

        public AcademicAnalysisRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AcademicAnalysis>> GetAllAsync()
        {
            return await _context.AcademicAnalyses
                .Include(a => a.Student)
                    .ThenInclude(s => s.User)
                .OrderByDescending(a => a.AnalysisDate)
                .ToListAsync();
        }

        public async Task<AcademicAnalysis?> GetByIdAsync(int id)
        {
            return await _context.AcademicAnalyses
                .Include(a => a.Student)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(a => a.AnalysisId == id);
        }

        public async Task<IEnumerable<AcademicAnalysis>> GetByStudentAsync(int studentId)
        {
            return await _context.AcademicAnalyses
                .Include(a => a.Student)
                    .ThenInclude(s => s.User)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.AnalysisDate)
                .ToListAsync();
        }

        public async Task<AcademicAnalysis?> GetLatestByStudentAsync(int studentId)
        {
            return await _context.AcademicAnalyses
                .Include(a => a.Student)
                    .ThenInclude(s => s.User)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.AnalysisDate)
                .FirstOrDefaultAsync();
        }

        public async Task<AcademicAnalysis> AddAsync(AcademicAnalysis analysis)
        {
            _context.AcademicAnalyses.Add(analysis);
            await _context.SaveChangesAsync();
            return analysis;
        }

        public async Task<AcademicAnalysis> UpdateAsync(AcademicAnalysis analysis)
        {
            _context.AcademicAnalyses.Update(analysis);
            await _context.SaveChangesAsync();
            return analysis;
        }

        public async Task DeleteAsync(int id)
        {
            var analysis = await _context.AcademicAnalyses.FindAsync(id);
            if (analysis != null)
            {
                _context.AcademicAnalyses.Remove(analysis);
                await _context.SaveChangesAsync();
            }
        }
    }
}
