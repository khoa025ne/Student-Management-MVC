using DataAccess.Entities;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Implementations
{
    /// <summary>
    /// Service xử lý Class - Logic nghiệp vụ lớp học
    /// </summary>
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;

        public ClassService(IClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        public async Task<IEnumerable<Class>> GetAllAsync()
        {
            try
            {
                return await _classRepository.GetAllAsync();
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
                return await _classRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy lớp học: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Class>> GetBySemesterAsync(int semesterId)
        {
            try
            {
                return await _classRepository.GetBySemesterAsync(semesterId);
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
                return await _classRepository.GetByCourseAsync(courseId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy lớp học theo môn: {ex.Message}", ex);
            }
        }

        public async Task<Class> CreateAsync(Class classEntity)
        {
            try
            {
                // Kiểm tra sĩ số tối đa
                if (classEntity.MaxCapacity <= 0)
                {
                    classEntity.MaxCapacity = 30; // Mặc định
                }

                return await _classRepository.AddAsync(classEntity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo lớp học: {ex.Message}", ex);
            }
        }

        public async Task<Class> UpdateAsync(Class classEntity)
        {
            try
            {
                return await _classRepository.UpdateAsync(classEntity);
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
                await _classRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa lớp học: {ex.Message}", ex);
            }
        }
    }
}
