using DataAccess;
using DataAccess.DAO;
using DataAccess.Entities;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    /// <summary>
    /// Repository cho Role - gọi xuống DAO
    /// </summary>
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleDAO _roleDAO;

        public RoleRepository(AppDbContext context)
        {
            _roleDAO = new RoleDAO(context);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _roleDAO.GetAllAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _roleDAO.GetByIdAsync(id);
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            return await _roleDAO.GetByNameAsync(roleName);
        }

        public async Task<Role> AddAsync(Role role)
        {
            return await _roleDAO.AddAsync(role);
        }
    }
}
