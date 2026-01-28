using DataAccess.Entities;
using Repositories.Interfaces;
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
                SemesterId = createDto.SemesterId,
                CourseId = createDto.CourseId,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                MaxStudents = createDto.MaxStudents,
                TeacherName = createDto.TeacherName,
                Description = createDto.Description,
                Schedule = createDto.Schedule,
                Location = createDto.Location,
                CurrentEnrollment = 0,
                IsActive = true
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
            classEntity.StartDate = updateDto.StartDate;
            classEntity.EndDate = updateDto.EndDate;
            classEntity.MaxStudents = updateDto.MaxStudents;
            classEntity.TeacherName = updateDto.TeacherName;
            classEntity.Description = updateDto.Description;
            classEntity.Schedule = updateDto.Schedule;
            classEntity.Location = updateDto.Location;
            classEntity.IsActive = updateDto.IsActive;

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
            var classes = await _classRepository.GetAllAsync();
            var activeClasses = classes.Where(c => c.IsActive);
            return activeClasses.Select(MapToDto);
        }

        public async Task<ClassDto?> UpdateClassStatusAsync(int classId, bool isActive)
        {
            var classEntity = await _classRepository.GetByIdAsync(classId);
            if (classEntity == null)
                return null;

            classEntity.IsActive = isActive;
            var updatedClass = await _classRepository.UpdateAsync(classEntity);
            return MapToDto(updatedClass);
        }

        public async Task<IEnumerable<ClassDto>> SearchClassesAsync(string searchTerm)
        {
            var classes = await _classRepository.GetAllAsync();
            var filtered = classes.Where(c => 
                c.ClassName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                c.TeacherName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                c.Location?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true);
            
            return filtered.Select(MapToDto);
        }

        private ClassDto MapToDto(Class classEntity)
        {
            return new ClassDto
            {
                ClassId = classEntity.ClassId,
                ClassName = classEntity.ClassName,
                SemesterId = classEntity.SemesterId,
                SemesterName = classEntity.Semester?.SemesterName,
                CourseId = classEntity.CourseId,
                CourseName = classEntity.Course?.CourseName,
                StartDate = classEntity.StartDate,
                EndDate = classEntity.EndDate,
                MaxStudents = classEntity.MaxStudents,
                CurrentEnrollment = classEntity.CurrentEnrollment,
                TeacherName = classEntity.TeacherName,
                Description = classEntity.Description,
                IsActive = classEntity.IsActive,
                Schedule = classEntity.Schedule,
                Location = classEntity.Location
            };
        }
    }
}