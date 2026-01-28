using DataAccess.Entities;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models;

namespace Services.Implementations
{
    /// <summary>
    /// Service xử lý Student - Logic nghiệp vụ sinh viên
    /// </summary>
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IEmailService _emailService;

        public StudentService(
            IStudentRepository studentRepository,
            IEnrollmentRepository enrollmentRepository,
            IEmailService emailService)
        {
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
            _emailService = emailService;
        }

        // ===== ENTITY-BASED METHODS (for complex operations needing navigation properties) =====
        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _studentRepository.GetAllAsync();
        }

        public async Task<Student?> GetByIdAsync(int studentId)
        {
            return await _studentRepository.GetByIdAsync(studentId);
        }

        public async Task<Student?> GetByUserIdAsync(int userId)
        {
            return await _studentRepository.GetByUserIdAsync(userId);
        }

        public async Task<Student?> GetStudentByCodeAsync(string studentCode)
        {
            return await _studentRepository.GetByCodeAsync(studentCode);
        }

        public async Task<Student> CreateAsync(Student student)
        {
            return await _studentRepository.AddAsync(student);
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            return await _studentRepository.UpdateAsync(student);
        }

        public async Task DeleteAsync(int studentId)
        {
            await _studentRepository.DeleteAsync(studentId);
        }

        // ===== DTO-BASED METHODS =====
        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return students.Select(MapToDto);
        }

        public async Task<StudentDto?> GetStudentDtoByIdAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            return student != null ? MapToDto(student) : null;
        }

        public async Task<StudentDto?> GetByCodeAsync(string studentCode)
        {
            var student = await _studentRepository.GetByCodeAsync(studentCode);
            return student != null ? MapToDto(student) : null;
        }

        public async Task<IEnumerable<StudentDto>> GetByMajorAsync(MajorType major)
        {
            var students = await _studentRepository.GetByMajorAsync(major);
            return students.Select(MapToDto);
        }

        public async Task<IEnumerable<StudentDto>> GetByClassCodeAsync(string classCode)
        {
            var students = await _studentRepository.GetByClassCodeAsync(classCode);
            return students.Select(MapToDto);
        }

        public async Task<StudentDto?> GetByEmailAsync(string email)
        {
            var students = await _studentRepository.GetAllAsync();
            var student = students.FirstOrDefault(s => s.Email == email);
            return student != null ? MapToDto(student) : null;
        }

        public async Task<StudentDto> CreateAsync(StudentCreateDto createDto)
        {
            var student = new Student
            {
                Email = createDto.Email,
                FullName = createDto.FullName,
                DateOfBirth = createDto.DateOfBirth ?? DateTime.Now,
                PhoneNumber = createDto.PhoneNumber,
                StudentCode = createDto.StudentCode ?? await GenerateStudentCodeAsync(),
                CreatedAt = DateTime.Now,
                Major = MajorType.Undefined,
                IsFirstLogin = true
            };

            var createdStudent = await _studentRepository.AddAsync(student);
            return MapToDto(createdStudent);
        }

        public async Task<StudentDto> UpdateAsync(StudentUpdateDto updateDto)
        {
            var student = await _studentRepository.GetByIdAsync(int.Parse(updateDto.StudentId));
            if (student == null)
                throw new Exception("Student not found");

            student.FullName = updateDto.FullName;
            student.DateOfBirth = updateDto.DateOfBirth ?? student.DateOfBirth;
            student.PhoneNumber = updateDto.PhoneNumber;

            var updatedStudent = await _studentRepository.UpdateAsync(student);
            return MapToDto(updatedStudent);
        }

        public async Task<bool> DeleteStudentAsync(int studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                return false;

            await _studentRepository.DeleteAsync(studentId);
            return true;
        }

        // ===== BUSINESS LOGIC METHODS =====
        public async Task<string> GenerateStudentCodeAsync()
        {
            var year = DateTime.Now.Year;
            var allStudents = await _studentRepository.GetAllAsync();
            
            var prefix = $"STU{year}";
            var studentsThisYear = allStudents
                .Where(s => s.StudentCode?.StartsWith(prefix) == true)
                .ToList();

            var maxNumber = 0;
            foreach (var student in studentsThisYear)
            {
                if (student.StudentCode?.Length >= 11)
                {
                    var numberPart = student.StudentCode.Substring(7);
                    if (int.TryParse(numberPart, out int number))
                    {
                        maxNumber = Math.Max(maxNumber, number);
                    }
                }
            }

            return $"{prefix}{(maxNumber + 1):D5}";
        }

        public string GenerateDefaultPassword(DateTime dateOfBirth)
        {
            return dateOfBirth.ToString("ddMMyyyy") + "@fpt";
        }

        public async Task<double> CalculateOverallGPAAsync(int studentId)
        {
            var enrollments = await _enrollmentRepository.GetByStudentAsync(studentId);
            var completedEnrollments = enrollments.Where(e => e.TotalScore.HasValue).ToList();

            if (!completedEnrollments.Any())
                return 0.0;

            return completedEnrollments.Average(e => e.TotalScore!.Value);
        }

        public async Task<StudentDto> CreateStudentWithUserAsync(StudentCreateDto createDto)
        {
            return await CreateAsync(createDto);
        }

        public async Task<IEnumerable<StudentDto>> SearchStudentsAsync(string searchTerm)
        {
            var students = await _studentRepository.GetAllAsync();
            var filtered = students.Where(s => 
                s.FullName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                s.StudentCode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true ||
                s.Email?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true);
            
            return filtered.Select(MapToDto);
        }

        public async Task<StudentDto?> UpdateStudentStatusAsync(int studentId, string status)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                return null;

            var updatedStudent = await _studentRepository.UpdateAsync(student);
            return MapToDto(updatedStudent);
        }

        private StudentDto MapToDto(Student student)
        {
            return new StudentDto
            {
                StudentId = student.StudentId.ToString(),
                Email = student.Email,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                PhoneNumber = student.PhoneNumber,
                MajorName = student.Major.ToString(),
                StudentCode = student.StudentCode,
                GPA = student.OverallGPA
            };
        }
    }
}
