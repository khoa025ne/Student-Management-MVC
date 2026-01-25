using DataAccess.Entities;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Implementations
{
    /// <summary>
    /// Service xử lý Course - Logic nghiệp vụ môn học
    /// </summary>
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            try
            {
                return await _courseRepository.GetAllAsync();
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
                return await _courseRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy môn học: {ex.Message}", ex);
            }
        }

        public async Task<Course?> GetByCodeAsync(string courseCode)
        {
            try
            {
                return await _courseRepository.GetByCodeAsync(courseCode);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy môn học theo mã: {ex.Message}", ex);
            }
        }

        public async Task<Course> CreateAsync(Course course)
        {
            try
            {
                // Kiểm tra mã môn học đã tồn tại
                var existing = await _courseRepository.GetByCodeAsync(course.CourseCode);
                if (existing != null)
                {
                    throw new Exception("Mã môn học đã tồn tại");
                }

                return await _courseRepository.AddAsync(course);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo môn học: {ex.Message}", ex);
            }
        }

        public async Task<Course> UpdateAsync(Course course)
        {
            try
            {
                return await _courseRepository.UpdateAsync(course);
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
                await _courseRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa môn học: {ex.Message}", ex);
            }
        }
    }
}
