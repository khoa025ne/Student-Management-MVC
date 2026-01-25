using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Services.Implementations
{
    /// <summary>
    /// Service xử lý Authentication - Logic nghiệp vụ đăng nhập
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStudentService _studentService;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IStudentService studentService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _studentService = studentService;
            _configuration = configuration;
        }

        /// <summary>
        /// Đăng nhập bằng email và password
        /// </summary>
        public async Task<User?> LoginAsync(string email, string password)
        {
            try
            {
                // Kiểm tra Admin từ appsettings.json trước
                var adminUser = await LoginAdminFromConfigAsync(email, password);
                if (adminUser != null)
                {
                    return adminUser;
                }

                // Nếu không phải Admin, kiểm tra trong Database
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return null;
                }

                // Verify password
                if (!VerifyPassword(password, user.PasswordHash))
                {
                    return null;
                }

                // Cập nhật LastLogin
                user.LastLogin = DateTime.Now;
                await _userRepository.UpdateAsync(user);

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đăng nhập: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Đăng nhập Admin từ appsettings.json
        /// </summary>
        public async Task<User?> LoginAdminFromConfigAsync(string email, string password)
        {
            try
            {
                var adminEmail = _configuration["AdminAccount:Email"];
                var adminPassword = _configuration["AdminAccount:Password"];

                if (email == adminEmail && password == adminPassword)
                {
                    // Trả về User giả lập cho Admin
                    return new User
                    {
                        UserId = 0,
                        Email = adminEmail,
                        FullName = "Administrator",
                        RoleId = 1, // Admin role
                        Role = new Role { RoleId = 1, RoleName = "Admin" },
                        IsActive = true,
                        LastLogin = DateTime.Now
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra Admin: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception("Người dùng không tồn tại");
                }

                // Verify old password
                if (!VerifyPassword(oldPassword, user.PasswordHash))
                {
                    throw new UnauthorizedAccessException("Mật khẩu cũ không chính xác");
                }

                // Update new password
                user.PasswordHash = HashPassword(newPassword);
                user.PasswordChangedAt = DateTime.Now;
                user.MustChangePassword = false;

                await _userRepository.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đổi mật khẩu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        public async Task<User> RegisterAsync(User user, string password)
        {
            try
            {
                // Kiểm tra email đã tồn tại
                if (await _userRepository.ExistsByEmailAsync(user.Email))
                {
                    throw new Exception("Email đã được sử dụng");
                }

                // Hash password
                user.PasswordHash = HashPassword(password);
                user.CreatedAt = DateTime.Now;
                user.IsActive = true;
                user.MustChangePassword = true; // Yêu cầu đổi password lần đầu

                // Tạo User
                var createdUser = await _userRepository.AddAsync(user);

                // Nếu là Student (RoleId = 3), tự động tạo Student record
                if (createdUser.RoleId == 3)
                {
                    var studentCode = await _studentService.GenerateStudentCodeAsync();
                    var defaultPassword = _studentService.GenerateDefaultPassword(DateTime.Now);
                    
                    var student = new Student
                    {
                        StudentCode = studentCode,
                        FullName = createdUser.FullName,
                        Email = createdUser.Email,
                        PhoneNumber = "0000000000", // Default phone number
                        DateOfBirth = DateTime.Now.AddYears(-20), // Default: 20 tuổi
                        ClassCode = "TEMP",
                        Major = MajorType.Undefined, // Default major
                        CurrentTermNo = 1,
                        IsFirstLogin = true,
                        UserId = createdUser.UserId,
                        CreatedAt = DateTime.Now
                    };

                    await _studentService.CreateAsync(student);
                }

                return createdUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đăng ký: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hash password bằng BCrypt
        /// </summary>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verify password với BCrypt
        /// </summary>
        public bool VerifyPassword(string password, string passwordHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, passwordHash);
            }
            catch
            {
                return false;
            }
        }
    }
}
