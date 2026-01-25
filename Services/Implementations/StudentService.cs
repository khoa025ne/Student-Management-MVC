using DataAccess.Entities;
using DataAccess.Enums;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementations
{
    /// <summary>
    /// Service xử lý Student - Logic nghiệp vụ sinh viên
    /// </summary>
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public StudentService(
            IStudentRepository studentRepository,
            IEnrollmentRepository enrollmentRepository,
            IUserService userService,
            IEmailService emailService)
        {
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
            _userService = userService;
            _emailService = emailService;
        }

        /// <summary>
        /// Tạo mã sinh viên tự động theo format: STU + Năm + Số thứ tự
        /// VD: STU202600123
        /// </summary>
        public async Task<string> GenerateStudentCodeAsync()
        {
            var year = DateTime.Now.Year;
            var allStudents = await _studentRepository.GetAllAsync();
            
            // Lọc các SV có mã bắt đầu bằng STU + năm hiện tại
            var prefix = $"STU{year}";
            var studentsThisYear = allStudents
                .Where(s => s.StudentCode.StartsWith(prefix))
                .ToList();

            // Lấy số thứ tự lớn nhất
            var maxNumber = 0;
            foreach (var student in studentsThisYear)
            {
                if (student.StudentCode.Length >= 11) // STU2026 + 5 số
                {
                    var numberPart = student.StudentCode.Substring(7); // Lấy 5 số cuối
                    if (int.TryParse(numberPart, out var num))
                    {
                        if (num > maxNumber) maxNumber = num;
                    }
                }
            }

            // Tăng lên 1
            var nextNumber = maxNumber + 1;
            return $"{prefix}{nextNumber:D5}"; // D5 = pad 5 chữ số: 00001
        }

        /// <summary>
        /// Tạo mật khẩu mặc định theo format: NgàySinh@fpt
        /// VD: Sinh ngày 20/05/2003 => 20052003@fpt
        /// </summary>
        public string GenerateDefaultPassword(DateTime dateOfBirth)
        {
            return $"{dateOfBirth:ddMMyyyy}@fpt";
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            try
            {
                return await _studentRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách sinh viên: {ex.Message}", ex);
            }
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            try
            {
                return await _studentRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sinh viên: {ex.Message}", ex);
            }
        }

        public async Task<Student?> GetByCodeAsync(string studentCode)
        {
            try
            {
                return await _studentRepository.GetByCodeAsync(studentCode);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sinh viên theo mã: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Student>> GetByMajorAsync(MajorType major)
        {
            try
            {
                return await _studentRepository.GetByMajorAsync(major);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sinh viên theo ngành: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Student>> GetByTermAsync(int termNo)
        {
            try
            {
                return await _studentRepository.GetByTermAsync(termNo);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sinh viên theo kỳ: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Student>> GetByClassCodeAsync(string classCode)
        {
            try
            {
                return await _studentRepository.GetByClassCodeAsync(classCode);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sinh viên theo lớp: {ex.Message}", ex);
            }
        }

        public async Task<Student?> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _studentRepository.GetByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy sinh viên theo UserId: {ex.Message}", ex);
            }
        }

        public async Task<Student> CreateAsync(Student student)
        {
            try
            {
                // Kiểm tra mã sinh viên đã tồn tại
                var existing = await _studentRepository.GetByCodeAsync(student.StudentCode);
                if (existing != null)
                {
                    throw new Exception("Mã sinh viên đã tồn tại");
                }

                student.CreatedAt = DateTime.Now;
                return await _studentRepository.AddAsync(student);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo sinh viên: {ex.Message}", ex);
            }
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            try
            {
                return await _studentRepository.UpdateAsync(student);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật sinh viên: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _studentRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa sinh viên: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tính Overall GPA (GPA tổng kết) của sinh viên
        /// Công thức: Σ(GPA_môn × Credits_môn) / Σ(Credits)
        /// </summary>
        public async Task<double> CalculateOverallGPAAsync(int studentId)
        {
            try
            {
                var enrollments = await _enrollmentRepository.GetByStudentAsync(studentId);

                // Lấy các môn đã có điểm
                var completedEnrollments = enrollments
                    .Where(e => e.TotalScore.HasValue && e.Class?.Course?.Credits != null && e.IsPassed)
                    .ToList();

                if (!completedEnrollments.Any())
                {
                    return 0.0;
                }

                double totalWeightedGPA = 0;
                int totalCredits = 0;

                foreach (var enrollment in completedEnrollments)
                {
                    var gpa = enrollment.TotalScore!.Value;
                    var credits = enrollment.Class!.Course!.Credits;

                    totalWeightedGPA += gpa * credits;
                    totalCredits += credits;
                }

                if (totalCredits == 0) return 0.0;

                var overallGPA = totalWeightedGPA / totalCredits;

                // Cập nhật vào Student
                var student = await _studentRepository.GetByIdAsync(studentId);
                if (student != null)
                {
                    student.OverallGPA = Math.Round(overallGPA, 2);
                    await _studentRepository.UpdateAsync(student);
                }

                return Math.Round(overallGPA, 2);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tính Overall GPA: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tạo sinh viên mới cùng với User account và gửi email chào mừng
        /// </summary>
        public async Task<Student> CreateStudentWithUserAsync(Student student, string fullName, string email, string? phoneNumber)
        {
            try
            {
                // 1. Kiểm tra tuổi (>= 16 và <= 60)
                var age = DateTime.Now.Year - student.DateOfBirth.Year;
                if (DateTime.Now < student.DateOfBirth.AddYears(age)) age--;
                
                if (age < 16 || age > 60)
                {
                    throw new Exception("Tuổi phải từ 16 đến 60");
                }

                // 2. Kiểm tra email đã tồn tại
                var existingUser = await _userService.GetByEmailAsync(email);
                if (existingUser != null)
                {
                    throw new Exception($"Email {email} đã được sử dụng");
                }

                // 3. Tự động sinh mã sinh viên
                student.StudentCode = await GenerateStudentCodeAsync();

                // 4. Tạo mật khẩu mặc định
                var defaultPassword = GenerateDefaultPassword(student.DateOfBirth);

                // 5. Tạo User account
                var newUser = new User
                {
                    FullName = fullName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                    RoleId = 3, // Student role
                    IsActive = true,
                    MustChangePassword = true, // Bắt buộc đổi mật khẩu lần đầu
                    CreatedAt = DateTime.Now
                };

                newUser = await _userService.CreateAsync(newUser);

                // 6. Gán UserId cho Student
                student.UserId = newUser.UserId;
                student.IsFirstLogin = true;
                student.CreatedAt = DateTime.Now;

                // 7. Tạo Student
                var createdStudent = await _studentRepository.AddAsync(student);

                // 8. Gửi email chào mừng (async, không chờ)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var emailSubject = "🎓 Chào mừng bạn đến với Student Compass!";
                        var emailBody = $@"
                            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                                <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center;'>
                                    <h1 style='color: white; margin: 0;'>🎓 Chào mừng đến Student Compass!</h1>
                                </div>
                                <div style='padding: 30px; background: #f5f5f5;'>
                                    <h2 style='color: #333;'>Xin chào {fullName}!</h2>
                                    <p style='font-size: 16px; color: #555;'>
                                        Chúc mừng bạn đã trở thành sinh viên của chúng tôi. Dưới đây là thông tin tài khoản của bạn:
                                    </p>
                                    
                                    <div style='background: white; padding: 20px; border-radius: 10px; margin: 20px 0;'>
                                        <p><strong>📧 Email:</strong> {email}</p>
                                        <p><strong>🔑 Mật khẩu tạm:</strong> <code style='background: #e3e3e3; padding: 5px 10px; border-radius: 5px;'>{defaultPassword}</code></p>
                                        <p><strong>🎫 Mã sinh viên:</strong> <code style='background: #e3e3e3; padding: 5px 10px; border-radius: 5px;'>{student.StudentCode}</code></p>
                                        <p><strong>🏫 Lớp:</strong> {student.ClassCode}</p>
                                    </div>

                                    <div style='background: #fff3cd; padding: 15px; border-left: 4px solid #ffc107; border-radius: 5px; margin: 20px 0;'>
                                        <p style='margin: 0; color: #856404;'>
                                            <strong>⚠️ Lưu ý:</strong> Bạn cần <strong>đổi mật khẩu</strong> ngay khi đăng nhập lần đầu.
                                        </p>
                                    </div>

                                    <div style='text-align: center; margin: 30px 0;'>
                                        <a href='http://localhost:5005/Auth/Login' 
                                           style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); 
                                                  color: white; 
                                                  padding: 15px 40px; 
                                                  text-decoration: none; 
                                                  border-radius: 25px; 
                                                  display: inline-block;
                                                  font-weight: bold;'>
                                            🚀 Đăng nhập ngay
                                        </a>
                                    </div>

                                    <p style='color: #777; font-size: 14px; text-align: center;'>
                                        Chúc bạn có một hành trình học tập thành công! 🌟
                                    </p>
                                </div>
                            </div>
                        ";

                        await _emailService.SendEmailAsync(email, emailSubject, emailBody);
                    }
                    catch (Exception emailEx)
                    {
                        // Log lỗi nhưng không throw để không ảnh hưởng đến việc tạo student
                        Console.WriteLine($"Lỗi gửi email: {emailEx.Message}");
                    }
                });

                return createdStudent;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo sinh viên: {ex.Message}", ex);
            }
        }
    }
}
