using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using Services.Interfaces;
using Services.Models;

namespace Services.Implementations
{
    /// <summary>
    /// Service xử lý Class - Logic nghiệp vụ lớp học
    /// </summary>
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly ICourseRepository _courseRepository;

        public ClassService(
            IClassRepository classRepository,
            ISemesterRepository semesterRepository,
            ICourseRepository courseRepository)
        {
            _classRepository = classRepository;
            _semesterRepository = semesterRepository;
            _courseRepository = courseRepository;
        }

        // ===== ENTITY-BASED METHODS =====
        public async Task<IEnumerable<Class>> GetAllClassesAsync()
        {
            return await _classRepository.GetAllAsync();
        }

        public async Task<Class?> GetClassByIdAsync(int classId)
        {
            return await _classRepository.GetByIdAsync(classId);
        }

        public async Task<IEnumerable<Class>> GetClassesBySemesterAsync(int semesterId)
        {
            return await _classRepository.GetBySemesterAsync(semesterId);
        }

        public async Task<Class> CreateClassAsync(Class classEntity)
        {
            return await _classRepository.AddAsync(classEntity);
        }

        public async Task<Class> UpdateClassAsync(Class classEntity)
        {
            return await _classRepository.UpdateAsync(classEntity);
        }

        // ===== DTO-BASED METHODS =====
        public async Task<IEnumerable<ClassDto>> GetAllAsync()
        {
            var classes = await _classRepository.GetAllAsync();
            return classes.Select(MapToDto);
        }

        public async Task<ClassDto?> GetByIdAsync(int classId)
        {
            var classEntity = await _classRepository.GetByIdAsync(classId);
            return classEntity != null ? MapToDto(classEntity) : null;
        }

        public async Task<IEnumerable<ClassDto>> GetBySemesterAsync(int semesterId)
        {
            var classes = await _classRepository.GetBySemesterAsync(semesterId);
            return classes.Select(MapToDto);
        }

        public async Task<IEnumerable<ClassDto>> GetByCourseAsync(int courseId)
        {
            var classes = await _classRepository.GetByCourseAsync(courseId);
            return classes.Select(MapToDto);
        }

        public async Task<ClassDto> CreateAsync(ClassCreateDto createDto)
        {
            var classEntity = new Class
            {
                ClassName = createDto.ClassName,
                ClassCode = createDto.ClassName, // Default same as name
                SemesterId = createDto.SemesterId,
                CourseId = createDto.CourseId,
                MaxCapacity = createDto.MaxStudents,
                MaxStudents = createDto.MaxStudents,
                CurrentEnrollment = 0,
                Schedule = createDto.Schedule,
                Room = createDto.Location,
                CreatedAt = DateTime.Now
            };

            var createdClass = await _classRepository.AddAsync(classEntity);
            return MapToDto(createdClass);
        }

        public async Task<ClassDto> UpdateAsync(ClassUpdateDto updateDto)
        {
            var classEntity = await _classRepository.GetByIdAsync(updateDto.ClassId);
            if (classEntity == null)
                throw new Exception("Class not found");

            classEntity.ClassName = updateDto.ClassName;
            classEntity.SemesterId = updateDto.SemesterId;
            classEntity.CourseId = updateDto.CourseId;
            classEntity.MaxCapacity = updateDto.MaxStudents;
            classEntity.MaxStudents = updateDto.MaxStudents;
            classEntity.Schedule = updateDto.Schedule;
            classEntity.Room = updateDto.Location;

            var updatedClass = await _classRepository.UpdateAsync(classEntity);
            return MapToDto(updatedClass);
        }

        public async Task<bool> DeleteAsync(int classId)
        {
            var classEntity = await _classRepository.GetByIdAsync(classId);
            if (classEntity == null)
                return false;

            await _classRepository.DeleteAsync(classId);
            return true;
        }

        public async Task<IEnumerable<ClassDto>> GetActiveClassesAsync()
        {
            // All classes are considered active for now
            var classes = await _classRepository.GetAllAsync();
            return classes.Select(MapToDto);
        }

        public async Task<ClassDto?> UpdateClassStatusAsync(int classId, bool isActive)
        {
            // Class entity doesn't have IsActive, return current state
            var classEntity = await _classRepository.GetByIdAsync(classId);
            if (classEntity == null)
                return null;

            return MapToDto(classEntity);
        }

        public async Task<IEnumerable<ClassDto>> SearchClassesAsync(string searchTerm)
        {
            var classes = await _classRepository.GetAllAsync();
            var filtered = classes.Where(c => 
                c.ClassName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                c.ClassCode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                c.Room?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true);
            
            return filtered.Select(MapToDto);
        }

        private ClassDto MapToDto(Class classEntity)
        {
            return new ClassDto
            {
                ClassId = classEntity.ClassId,
                ClassName = classEntity.ClassName,
                ClassCode = classEntity.ClassCode,
                SemesterId = classEntity.SemesterId,
                SemesterName = classEntity.Semester?.SemesterName,
                CourseId = classEntity.CourseId,
                CourseName = classEntity.Course?.CourseName,
                MaxStudents = classEntity.MaxCapacity,
                MaxCapacity = classEntity.MaxCapacity,
                CurrentEnrollment = classEntity.CurrentEnrollment,
                TeacherId = classEntity.TeacherId,
                TeacherName = classEntity.Teacher?.User?.FullName,
                IsActive = true,
                Schedule = classEntity.Schedule,
                Location = classEntity.Room,
                Room = classEntity.Room,
                DayOfWeekPair = classEntity.DayOfWeekPair,
                TimeSlot = classEntity.TimeSlot,
                CreatedAt = classEntity.CreatedAt
            };
        }
    }
}
