using DataAccess.Entities;
using DataAccess.Enums;
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

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return students.Select(MapToDto);
        }

        public async Task<StudentDto?> GetByIdAsync(string studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            return student != null ? MapToDto(student) : null;
        }

        public async Task<StudentDto?> GetByCodeAsync(string studentCode)
        {
            var student = await _studentRepository.GetByCodeAsync(studentCode);
            return student != null ? MapToDto(student) : null;
        }

        public async Task<IEnumerable<StudentDto>> GetByMajorAsync(int majorId)
        {
            var students = await _studentRepository.GetAllAsync();
            var filtered = students.Where(s => s.MajorId == majorId);
            return filtered.Select(MapToDto);
        }

        public async Task<IEnumerable<StudentDto>> GetByClassIdAsync(int classId)
        {
            var students = await _studentRepository.GetAllAsync();
            var filtered = students.Where(s => s.ClassId == classId);
            return filtered.Select(MapToDto);
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
                StudentId = Guid.NewGuid().ToString(),
                Email = createDto.Email,
                FullName = createDto.FullName,
                DateOfBirth = createDto.DateOfBirth,
                PhoneNumber = createDto.PhoneNumber,
                Address = createDto.Address,
                MajorId = createDto.MajorId,
                ClassId = createDto.ClassId,
                Avatar = createDto.Avatar,
                StudentCode = createDto.StudentCode ?? await GenerateStudentCodeAsync(),
                ParentName = createDto.ParentName,
                ParentPhone = createDto.ParentPhone,
                ParentEmail = createDto.ParentEmail,
                EnrollmentDate = DateTime.Now,
                Status = "Active",
                GPA = 0.0,
                TotalCredits = 0
            };

            var createdStudent = await _studentRepository.AddAsync(student);
            return MapToDto(createdStudent);
        }

        public async Task<StudentDto> UpdateAsync(StudentUpdateDto updateDto)
        {
            var student = await _studentRepository.GetByIdAsync(updateDto.StudentId);
            if (student == null)
                throw new Exception("Student not found");

            student.FullName = updateDto.FullName;
            student.DateOfBirth = updateDto.DateOfBirth;
            student.PhoneNumber = updateDto.PhoneNumber;
            student.Address = updateDto.Address;
            student.MajorId = updateDto.MajorId;
            student.ClassId = updateDto.ClassId;
            student.Avatar = updateDto.Avatar;
            student.ParentName = updateDto.ParentName;
            student.ParentPhone = updateDto.ParentPhone;
            student.ParentEmail = updateDto.ParentEmail;
            student.Status = updateDto.Status;

            var updatedStudent = await _studentRepository.UpdateAsync(student);
            return MapToDto(updatedStudent);
        }

        public async Task<bool> DeleteAsync(string studentId)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                return false;

            await _studentRepository.DeleteAsync(studentId);
            return true;
        }

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
            return dateOfBirth.ToString("ddMMyyyy");
        }

        public async Task<double> CalculateOverallGPAAsync(string studentId)
        {
            var enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId);
            var completedEnrollments = enrollments.Where(e => e.FinalGrade.HasValue).ToList();

            if (!completedEnrollments.Any())
                return 0.0;

            return completedEnrollments.Average(e => e.FinalGrade!.Value);
        }

        public async Task<StudentDto> CreateStudentWithUserAsync(StudentCreateDto createDto)
        {
            // This method would typically coordinate with UserService to create both User and Student
            // For now, just create the student
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

        public async Task<StudentDto?> UpdateStudentStatusAsync(string studentId, string status)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null)
                return null;

            student.Status = status;
            var updatedStudent = await _studentRepository.UpdateAsync(student);
            return MapToDto(updatedStudent);
        }

        private StudentDto MapToDto(Student student)
        {
            return new StudentDto
            {
                StudentId = student.StudentId,
                Email = student.Email,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                PhoneNumber = student.PhoneNumber,
                Address = student.Address,
                MajorId = student.MajorId,
                MajorName = student.Major?.MajorName, // Assuming Major navigation property
                ClassId = student.ClassId,
                ClassName = student.Class?.ClassName, // Assuming Class navigation property
                Avatar = student.Avatar,
                EnrollmentDate = student.EnrollmentDate,
                Status = student.Status ?? "Active",
                StudentCode = student.StudentCode,
                ParentName = student.ParentName,
                ParentPhone = student.ParentPhone,
                ParentEmail = student.ParentEmail,
                GPA = student.GPA,
                TotalCredits = student.TotalCredits
            };
        }
    }
}