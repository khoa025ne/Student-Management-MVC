using DataAccess.Entities;
using Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    /// <summary>
    /// Interface cho User Service
    /// </summary>
    public interface IUserService
    {
        // Entity-based methods (for internal use by other services)
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<bool> ExistsByEmailAsync(string email);
        
        // DTO-based methods (for Controllers)
        Task<UserDto> CreateAsync(UserCreateDto dto, bool hashPassword = true);
        Task<UserDto> UpdateDtoAsync(UserUpdateDto dto);
    }
}
