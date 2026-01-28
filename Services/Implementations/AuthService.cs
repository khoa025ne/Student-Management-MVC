using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Models;

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
        public async Task<UserDto?> LoginAsync(string email, string password)
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

                // Cập nhật thời gian đăng nhập cuối
                await UpdateLastLoginAsync(user.UserId);

                return MapToUserDto(user);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Đăng nhập Admin từ appsettings.json
        /// </summary>
        public async Task<UserDto?> LoginAdminFromConfigAsync(string email, string password)
        {
            try
            {
                var adminEmail = _configuration["AdminAccount:Email"];
                var adminPassword = _configuration["AdminAccount:Password"];

                if (email == adminEmail && password == adminPassword)
                {
                    // Trả về UserDto cho Admin
                    return new UserDto
                    {
                        UserId = "admin-0",
                        Email = adminEmail ?? "admin@example.com",
                        FullName = "Administrator",
                        IsActive = true,
                        LastLogin = DateTime.Now,
                        Roles = new List<RoleDto>
                        {
                            new RoleDto { RoleId = 1, RoleName = "Admin" }
                        }
                    };
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        public async Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
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
        public async Task<UserDto> RegisterAsync(UserCreateDto userCreateDto, string password)
        {
            try
            {
                // Kiểm tra email đã tồn tại
                if (await _userRepository.ExistsByEmailAsync(userCreateDto.Email))
                {
                    throw new Exception("Email đã được sử dụng");
                }

                // Tạo User entity từ DTO
                var user = new User
                {
                    Email = userCreateDto.Email,
                    FullName = userCreateDto.FullName,
                    DateOfBirth = userCreateDto.DateOfBirth,
                    PhoneNumber = userCreateDto.PhoneNumber,
                    Address = userCreateDto.Address,
                    Avatar = userCreateDto.Avatar,
                    PasswordHash = HashPassword(password),
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                    MustChangePassword = true // Yêu cầu đổi password lần đầu
                };

                // Gán role đầu tiên nếu có
                if (userCreateDto.RoleIds.Any())
                {
                    user.RoleId = userCreateDto.RoleIds.First();
                }

                // Tạo User
                var createdUser = await _userRepository.AddAsync(user);

                // Nếu là Student (RoleId = 3), tự động tạo Student record
                if (createdUser.RoleId == 3)
                {
                    var studentCreateDto = new StudentCreateDto
                    {
                        Email = createdUser.Email,
                        FullName = createdUser.FullName,
                        DateOfBirth = createdUser.DateOfBirth,
                        PhoneNumber = createdUser.PhoneNumber,
                        Address = createdUser.Address,
                        Avatar = createdUser.Avatar
                    };

                    await _studentService.CreateStudentWithUserAsync(studentCreateDto);
                }

                return MapToUserDto(createdUser);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đăng ký: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Hash password using BCrypt
        /// </summary>
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verify password using BCrypt
        /// </summary>
        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        /// <summary>
        /// Lấy thông tin user theo email
        /// </summary>
        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                return user != null ? MapToUserDto(user) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Cập nhật thời gian đăng nhập cuối
        /// </summary>
        public async Task<bool> UpdateLastLoginAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    user.LastLogin = DateTime.Now;
                    await _userRepository.UpdateAsync(user);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Mapping từ User Entity sang UserDto
        /// </summary>
        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Avatar = user.Avatar,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                IsEmailConfirmed = user.IsEmailConfirmed,
                IsPhoneConfirmed = user.IsPhoneConfirmed,
                IsActive = user.IsActive,
                Roles = user.Role != null ? new List<RoleDto>
                {
                    new RoleDto
                    {
                        RoleId = user.Role.RoleId,
                        RoleName = user.Role.RoleName,
                        Description = user.Role.Description,
                        Color = user.Role.Color,
                        IsActive = user.Role.IsActive
                    }
                } : new List<RoleDto>()
            };
        }
    }
}
