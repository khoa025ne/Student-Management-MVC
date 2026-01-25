using DataAccess.Entities;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Implementations
{
    /// <summary>
    /// Service xử lý Semester - Logic nghiệp vụ học kỳ
    /// </summary>
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;

        public SemesterService(ISemesterRepository semesterRepository)
        {
            _semesterRepository = semesterRepository;
        }

        public async Task<IEnumerable<Semester>> GetAllAsync()
        {
            try
            {
                return await _semesterRepository.GetAllAsync();
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
                return await _semesterRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy học kỳ: {ex.Message}", ex);
            }
        }

        public async Task<Semester?> GetActiveAsync()
        {
            try
            {
                return await _semesterRepository.GetActiveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy học kỳ hiện tại: {ex.Message}", ex);
            }
        }

        public async Task<Semester> CreateAsync(Semester semester)
        {
            try
            {
                return await _semesterRepository.AddAsync(semester);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo học kỳ: {ex.Message}", ex);
            }
        }

        public async Task<Semester> UpdateAsync(Semester semester)
        {
            try
            {
                return await _semesterRepository.UpdateAsync(semester);
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
                await _semesterRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa học kỳ: {ex.Message}", ex);
            }
        }
    }
}
