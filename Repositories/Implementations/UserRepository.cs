using DataAccess;
using DataAccess.DAO;
using DataAccess.Entities;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    /// <summary>
    /// Repository cho User - gọi xuống DAO
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO _userDAO;

        public UserRepository(AppDbContext context)
        {
            // Sử dụng Singleton Pattern từ DAO
            _userDAO = new UserDAO(context);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userDAO.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userDAO.GetByIdAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userDAO.GetByEmailAsync(email);
        }

        public async Task<User> AddAsync(User user)
        {
            return await _userDAO.AddAsync(user);
        }

        public async Task<User> UpdateAsync(User user)
        {
            return await _userDAO.UpdateAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            await _userDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _userDAO.ExistsByEmailAsync(email);
        }
    }
}
