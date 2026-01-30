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
    /// Service xử lý User - Logic nghiệp vụ người dùng
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _userRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách người dùng: {ex.Message}", ex);
            }
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _userRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy người dùng: {ex.Message}", ex);
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                return await _userRepository.GetByEmailAsync(email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy người dùng theo email: {ex.Message}", ex);
            }
        }

        public async Task<User> CreateAsync(User user)
        {
            try
            {
                // Kiểm tra email đã tồn tại
                if (await _userRepository.ExistsByEmailAsync(user.Email))
                {
                    throw new Exception("Email đã được sử dụng");
                }

                user.CreatedAt = DateTime.Now;
                return await _userRepository.AddAsync(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo người dùng: {ex.Message}", ex);
            }
        }

        public async Task<User> UpdateAsync(User user)
        {
            try
            {
                return await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật người dùng: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _userRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa người dùng: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            try
            {
                return await _userRepository.ExistsByEmailAsync(email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra email: {ex.Message}", ex);
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // DTO-BASED METHODS (for Controllers)
        // ═══════════════════════════════════════════════════════════════

        public async Task<UserDto> CreateAsync(UserCreateDto dto, bool hashPassword = true)
        {
            try
            {
                // Kiểm tra email đã tồn tại
                if (await _userRepository.ExistsByEmailAsync(dto.Email))
                {
                    throw new Exception("Email đã được sử dụng");
                }

                var user = new User
                {
                    Email = dto.Email,
                    FullName = dto.FullName,
                    PhoneNumber = dto.PhoneNumber,
                    PasswordHash = hashPassword ? BCrypt.Net.BCrypt.HashPassword(dto.Password) : dto.Password,
                    RoleId = dto.RoleIds.Count > 0 ? dto.RoleIds[0] : 4, // Default to Student role
                    IsActive = true,
                    MustChangePassword = true,
                    CreatedAt = DateTime.Now
                };

                if (!string.IsNullOrEmpty(dto.Avatar))
                {
                    user.AvatarUrl = dto.Avatar;
                }

                var created = await _userRepository.AddAsync(user);

                return new UserDto
                {
                    UserIdInt = created.UserId,
                    UserId = created.UserId.ToString(),
                    Email = created.Email,
                    FullName = created.FullName,
                    PhoneNumber = created.PhoneNumber,
                    AvatarUrl = created.AvatarUrl,
                    IsActive = created.IsActive,
                    RoleId = created.RoleId,
                    CreatedAt = created.CreatedAt,
                    MustChangePassword = created.MustChangePassword
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo người dùng: {ex.Message}", ex);
            }
        }

        public async Task<UserDto> UpdateDtoAsync(UserUpdateDto dto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(dto.UserId);
                if (user == null)
                {
                    throw new Exception("Không tìm thấy người dùng");
                }

                user.Email = dto.Email;
                user.FullName = dto.FullName;
                user.PhoneNumber = dto.PhoneNumber;
                user.IsActive = dto.IsActive;
                user.RoleId = dto.RoleId;
                if (!string.IsNullOrEmpty(dto.Avatar))
                {
                    user.AvatarUrl = dto.Avatar;
                }

                var updated = await _userRepository.UpdateAsync(user);
                
                return new UserDto
                {
                    UserIdInt = updated.UserId,
                    UserId = updated.UserId.ToString(),
                    Email = updated.Email,
                    FullName = updated.FullName,
                    PhoneNumber = updated.PhoneNumber,
                    AvatarUrl = updated.AvatarUrl,
                    IsActive = updated.IsActive,
                    RoleId = updated.RoleId,
                    CreatedAt = updated.CreatedAt,
                    LastLogin = updated.LastLogin
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật người dùng: {ex.Message}", ex);
            }
        }
    }
}
