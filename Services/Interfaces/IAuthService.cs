using Services.Models;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Authentication Service
    /// </summary>
    public interface IAuthService
    {
        Task<UserDto?> LoginAsync(string email, string password);
        Task<UserDto?> LoginAdminFromConfigAsync(string email, string password);
        Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword);
        Task<UserDto> RegisterAsync(UserCreateDto userCreateDto, string password);
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<bool> UpdateLastLoginAsync(string userId);
    }
}
