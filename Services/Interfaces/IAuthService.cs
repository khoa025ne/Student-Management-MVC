using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho Authentication Service
    /// </summary>
    public interface IAuthService
    {
        Task<User?> LoginAsync(string email, string password);
        Task<User?> LoginAdminFromConfigAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<User> RegisterAsync(User user, string password);
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}
