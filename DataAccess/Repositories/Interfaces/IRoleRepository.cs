using DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Interfaces
{
    /// <summary>
    /// Interface cho Role Repository
    /// </summary>
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByNameAsync(string roleName);
        Task<Role> AddAsync(Role role);
    }
}
