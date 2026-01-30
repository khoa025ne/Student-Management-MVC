using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Services.Interfaces;
using Services.Models;
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

        // ═══════════════════════════════════════════════════════════════
        // DTO-BASED METHODS (for Controllers)
        // ═══════════════════════════════════════════════════════════════

        public async Task<SemesterDto> CreateDtoAsync(SemesterCreateDto dto)
        {
            try
            {
                var semester = new Semester
                {
                    SemesterName = dto.SemesterName,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = false // Default inactive, admin can activate later
                };

                var created = await _semesterRepository.AddAsync(semester);
                
                return new SemesterDto
                {
                    SemesterId = created.SemesterId,
                    SemesterName = created.SemesterName,
                    StartDate = created.StartDate,
                    EndDate = created.EndDate,
                    IsActive = created.IsActive
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo học kỳ: {ex.Message}", ex);
            }
        }

        public async Task<SemesterDto> UpdateDtoAsync(SemesterUpdateDto dto)
        {
            try
            {
                var semester = await _semesterRepository.GetByIdAsync(dto.SemesterId);
                if (semester == null)
                {
                    throw new Exception("Không tìm thấy học kỳ");
                }

                semester.SemesterName = dto.SemesterName;
                semester.StartDate = dto.StartDate;
                semester.EndDate = dto.EndDate;
                semester.IsActive = dto.IsActive;

                var updated = await _semesterRepository.UpdateAsync(semester);
                
                return new SemesterDto
                {
                    SemesterId = updated.SemesterId,
                    SemesterName = updated.SemesterName,
                    StartDate = updated.StartDate,
                    EndDate = updated.EndDate,
                    IsActive = updated.IsActive
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật học kỳ: {ex.Message}", ex);
            }
        }
    }
}
