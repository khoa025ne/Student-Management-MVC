using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class ScoreService : IScoreService
    {
        private readonly IScoreRepository _scoreRepo;

        public ScoreService(IScoreRepository scoreRepo)
        {
            _scoreRepo = scoreRepo;
        }

        public async Task<IEnumerable<Score>> GetAllAsync()
        {
            return await _scoreRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Score>> GetByStudentIdAsync(int studentId)
        {
            return await _scoreRepo.GetByStudentIdAsync(studentId);
        }

        public async Task<Score?> GetByIdAsync(int id)
        {
            return await _scoreRepo.GetByIdAsync(id);
        }

        public async Task<Score> AddOrUpdateScoreAsync(int studentId, int courseId, double scoreValue)
        {
            var existingScore = await _scoreRepo.GetByStudentAndCourseAsync(studentId, courseId);
            if (existingScore != null)
            {
                existingScore.ScoreValue = scoreValue;
                return await _scoreRepo.UpdateAsync(existingScore);
            }
            else
            {
                var newScore = new Score
                {
                    StudentId = studentId,
                    CourseId = courseId,
                    ScoreValue = scoreValue
                };
                return await _scoreRepo.AddAsync(newScore);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _scoreRepo.DeleteAsync(id);
        }
    }
}

